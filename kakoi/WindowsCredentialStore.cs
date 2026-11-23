using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace kakoi
{
    /// <summary>
    /// Windows 資格情報マネージャー（advapi32.dll）への薄い P/Invoke ラッパー。
    /// 従来の CredentialManagement 1.0.2 と同一形式（CRED_TYPE_GENERIC / CRED_PERSIST_LOCAL_MACHINE / UTF-16LE）で互換性を維持する。
    /// </summary>
    internal static class WindowsCredentialStore
    {
        private const uint CRED_TYPE_GENERIC = 1;
        private const uint CRED_PERSIST_LOCAL_MACHINE = 2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDENTIAL
        {
            public uint Flags;
            public uint Type;
            public string TargetName;
            public string? Comment;
            public FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public string? TargetAlias;
            public string? UserName;
        }

        [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredWrite(ref CREDENTIAL userCredential, uint flags);

        [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredRead(string target, uint type, uint reservedFlag, out IntPtr credentialPtr);

        [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredDelete(string target, uint type, uint flags);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern void CredFree(IntPtr buffer);

        public static bool Save(string target, string? username, string secret)
        {
            var secretBytes = Encoding.Unicode.GetBytes(secret ?? string.Empty);
            var blobPtr = IntPtr.Zero;
            try
            {
                if (secretBytes.Length > 0)
                {
                    blobPtr = Marshal.AllocHGlobal(secretBytes.Length);
                    Marshal.Copy(secretBytes, 0, blobPtr, secretBytes.Length);
                }

                var credential = new CREDENTIAL
                {
                    Type = CRED_TYPE_GENERIC,
                    TargetName = target,
                    UserName = username,
                    CredentialBlob = blobPtr,
                    CredentialBlobSize = (uint)secretBytes.Length,
                    Persist = CRED_PERSIST_LOCAL_MACHINE
                };

                return CredWrite(ref credential, 0);
            }
            finally
            {
                if (blobPtr != IntPtr.Zero)
                {
                    // メモリ上の秘密情報をゼロクリアしてから解放
                    for (int i = 0; i < secretBytes.Length; i++)
                    {
                        Marshal.WriteByte(blobPtr, i, 0);
                    }
                    Marshal.FreeHGlobal(blobPtr);
                }
                Array.Clear(secretBytes, 0, secretBytes.Length);
            }
        }

        public static string? Load(string target)
        {
            if (!CredRead(target, CRED_TYPE_GENERIC, 0, out var credPtr))
            {
                return null;
            }

            try
            {
                var credential = Marshal.PtrToStructure<CREDENTIAL>(credPtr);
                if (credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize == 0)
                {
                    return string.Empty;
                }

                return Marshal.PtrToStringUni(credential.CredentialBlob, (int)(credential.CredentialBlobSize / 2))?.TrimEnd('\0')
                    ?? string.Empty;
            }
            finally
            {
                CredFree(credPtr);
            }
        }

        public static bool Delete(string target)
        {
            return CredDelete(target, CRED_TYPE_GENERIC, 0);
        }
    }
}
