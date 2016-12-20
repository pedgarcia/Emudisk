//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal static class NativeMethods
    {
        #region Constants

        #region Key definitions

        /// <summary>
        /// Posted to the window with the keyboard focus when a non-system key is pressed. A non-system key is a key that is pressed when the ALT key is not pressed. 
        /// </summary>
        internal const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// Posted to the window with the keyboard focus when a non-system key is released. A non-system key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus. 
        /// </summary>
        internal const int WM_KEYUP = 0x101;

        /// <summary>
        /// Posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_CHAR message contains the character code of the key that was pressed. 
        /// </summary>
        internal const int WM_CHAR = 0x102;

        // Clipboard formats used for cut/copy/drag operations

        /// <summary>
        /// This format identifier is used by the source to specify whether its preferred method of data transfer is move or copy.
        /// </summary>
        internal const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";

        /// <summary>
        /// This format identifier is used by the target to inform the data object through its IDataObject::SetData method of the outcome of a data transfer.
        /// </summary>
        internal const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";

        /// <summary>
        /// This format identifier is used with the CFSTR_FILECONTENTS format to transfer data as a group of files.
        /// </summary>
        internal const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";

        /// <summary>
        /// This format identifier is used with the CFSTR_FILEDESCRIPTOR format to transfer data as if it were a file, regardless of how it is actually stored.
        /// </summary>
        internal const string CFSTR_FILECONTENTS = "FileContents";

        #endregion

        #region File Descriptor Flags

        /// <summary>
        /// The clsid member is valid.
        /// </summary>
        internal const int FD_CLSID = 0x00000001;

        /// <summary>
        /// The sizel and pointl members are valid.
        /// </summary>
        internal const int FD_SIZEPOINT = 0x00000002;

        /// <summary>
        /// The dwFileAttributes member is valid.
        /// </summary>
        internal const int FD_ATTRIBUTES = 0x00000004;

        /// <summary>
        /// The ftCreationTime member is valid.
        /// </summary>
        internal const int FD_CREATETIME = 0x00000008;

        /// <summary>
        /// The ftLastAccessTime member is valid.
        /// </summary>
        internal const int FD_ACCESSTIME = 0x00000010;

        /// <summary>
        /// The ftLastWriteTime member is valid.
        /// </summary>
        internal const int FD_WRITESTIME = 0x00000020;

        /// <summary>
        /// The nFileSizeHigh and nFileSizeLow members are valid.
        /// </summary>
        internal const int FD_FILESIZE = 0x00000040;

        /// <summary>
        /// A progress indicator is shown with drag-and-drop operations.
        /// </summary>
        internal const int FD_PROGRESSUI = 0x00004000;

        /// <summary>
        /// Treat the operation as a shortcut.
        /// </summary>
        internal const int FD_LINKUI = 0x00008000;

        /// <summary>
        /// The handle that identifies a directory.
        /// </summary>
        internal const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        /// <summary>
        /// A file or directory that is an archive file or directory.
        /// </summary>
        internal const int FILE_ATTRIBUTE_ARCHIVE = 0x00000020;

        /// <summary>
        /// A file that does not have other attributes set.
        /// </summary>
        internal const int FILE_ATTRIBUTE_NORMAL = 0x00000080;

        #endregion

        #region Global Memory Flags

        /// <summary>
        /// Allocates movable memory.
        /// </summary>
        internal const int GMEM_MOVEABLE = 0x0002;

        /// <summary>
        /// Initializes memory contents to zero.
        /// </summary>
        internal const int GMEM_ZEROINIT = 0x0040;

        /// <summary>
        /// Combines GMEM_MOVEABLE and GMEM_ZEROINIT.
        /// </summary>
        internal const int GHND = GMEM_MOVEABLE | GMEM_ZEROINIT;

        /// <summary>
        /// Obsolete, but are provided for compatibility with 16-bit Windows.
        /// </summary>
        internal const int GMEM_DDESHARE = 0x2000;

        #endregion

        #region IDataObject constants

        /// <summary>
        /// The tymed value is not valid.
        /// </summary>
        internal const int DV_E_TYMED = unchecked((int)0x80040069);

        #endregion

        #region CreateFile constants

        /// <summary>
        /// Read access
        /// </summary>
        internal const uint GENERIC_READ = 0x80000000;

        /// <summary>
        /// Write Access
        /// </summary>
        internal const uint GENERIC_WRITE = 0x40000000;

        /// <summary>
        /// Opens a file or device, only if it exists.
        /// </summary>
        internal const uint OPEN_EXISTING = 0x03;

        #endregion

        #region MessageBoxManager constants

        /// <summary>
        /// The WH_CALLWNDPROC and WH_CALLWNDPROCRET hooks enable you to monitor messages sent to window procedures.
        /// </summary>
        internal const int WH_CALLWNDPROCRET = 12;

        /// <summary>
        /// Sent when a window is being destroyed.
        /// </summary>
        internal const int WM_DESTROY = 0x0002;

        /// <summary>
        /// Sent to the dialog box procedure immediately before a dialog box is displayed.
        /// </summary>
        internal const int WM_INITDIALOG = 0x0110;

        /// <summary>
        /// Posted to the installing thread's message queue when a timer expires.
        /// </summary>
        internal const int WM_TIMER = 0x0113;

        /// <summary>
        /// Used to define private messages for use by private window classes, usually of the form WM_USER+x, where x is an integer value.
        /// </summary>
        internal const int WM_USER = 0x400;

        /// <summary>
        /// Retrieves the identifier of the default push button control for a dialog box. 
        /// </summary>
        internal const int DM_GETDEFID = WM_USER + 0;

        /// <summary>
        /// OK Button
        /// </summary>
        internal const int MBOK = 1;

        /// <summary>
        /// Cancel Button
        /// </summary>
        internal const int MBCancel = 2;

        /// <summary>
        /// Abort Button
        /// </summary>
        internal const int MBAbort = 3;

        /// <summary>
        /// Retry Button
        /// </summary>
        internal const int MBRetry = 4;

        /// <summary>
        /// Ignore Button
        /// </summary>
        internal const int MBIgnore = 5;

        /// <summary>
        /// Yes Button
        /// </summary>
        internal const int MBYes = 6;

        /// <summary>
        /// No Button
        /// </summary>
        internal const int MBNo = 7;

        #endregion

        #region DataObjectEx structures

        /// <summary>
        /// TYMEDs allowed for DataObjectEx
        /// </summary>
        internal static readonly TYMED[] ALLOWED_TYMEDS = new TYMED[] { TYMED.TYMED_ENHMF, TYMED.TYMED_GDI, TYMED.TYMED_HGLOBAL, TYMED.TYMED_ISTREAM, TYMED.TYMED_MFPICT };

        #endregion

        #endregion

        #region Delegates

        /// <summary>
        /// HookProc Delegate
        /// </summary>
        /// <param name="nCode">Specifies whether the hook procedure must process the message.</param>
        /// <param name="wParam">Specifies whether the message has been removed from the queue.</param>
        /// <param name="lParam">A pointer to an MSG structure that contains details about the message.</param>
        /// <returns>A pointer to an MSG structure that contains details about the message. If code is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise, other applications that have installed WH_GETMESSAGE hooks will not receive hook notifications and may behave incorrectly as a result.</returns>
        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// An application-defined callback function used with the EnumChildWindows function.
        /// </summary>
        /// <param name="hWnd">A handle to a child window of the parent window specified in EnumChildWindows.</param>
        /// <param name="lParam">The application-defined value given in EnumChildWindows. </param>
        /// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE. </returns>
        internal delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        #endregion

        #region Native Methods

        /// <summary>
        /// Creates or opens a file, file stream, directory, physical disk, volume, console buffer, tape drive, communications resource, mailslot, or named pipe.
        /// </summary>
        /// <param name="fileName">The name of the object to be created or opened.</param>
        /// <param name="fileAccess">The access to the object, which can be read, write, or both.</param>
        /// <param name="fileShare">The sharing mode of an object, which can be read, write, both, or none.</param>
        /// <param name="securityAttributes">A pointer to a SECURITY_ATTRIBUTES structure that determines whether or not the returned handle can be inherited by child processes.</param>
        /// <param name="creationDisposition">An action to take on files that exist and do not exist.</param>
        /// <param name="flags">The file attributes and flags.</param>
        /// <param name="template">A handle to a template file with the GENERIC_READ access right.</param>
        /// <returns>If the function succeeds, the return value is an open handle to a specified file.</returns>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(
            string fileName,
            uint fileAccess,
            uint fileShare,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flags,
            IntPtr template);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="device">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(
            SafeFileHandle device);

        /// <summary>
        /// Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.
        /// </summary>
        /// <param name="device">A handle to the device on which the operation is to be performed.</param>
        /// <param name="controlCode">The control code for the operation.</param>
        /// <param name="inBuffer">A pointer to the input buffer that contains the data required to perform the operation.</param>
        /// <param name="inBufferSize">The size of the input buffer, in bytes.</param>
        /// <param name="outBuffer">A pointer to the output buffer that is to receive the data returned by the operation.</param>
        /// <param name="outBufferSize">The size of the output buffer, in bytes.</param>
        /// <param name="bytesReturned">A pointer to a variable that receives the size of the data stored in the output buffer, in bytes.</param>
        /// <param name="overlapped">A pointer to an OVERLAPPED structure.</param>
        /// <returns>If the operation completes successfully, the return value is nonzero.</returns>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeviceIoControl(
            SafeFileHandle device,
            uint controlCode,
            IntPtr inBuffer,
            uint inBufferSize,
            IntPtr outBuffer,
            uint outBufferSize,
            ref uint bytesReturned,
            IntPtr overlapped);

        /// <summary>
        /// Allocates the specified number of bytes from the heap.
        /// </summary>
        /// <param name="uFlags">The memory allocation attributes.</param>
        /// <param name="dwBytes">The number of bytes to allocate.</param>
        /// <returns>If the function succeeds, the return value is a handle to the newly allocated memory object.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GlobalAlloc(
            int uFlags,
            UIntPtr dwBytes);

        /// <summary>
        /// Frees the specified global memory object and invalidates its handle.
        /// </summary>
        /// <param name="handle">A handle to the global memory object.</param>
        /// <returns>If the function succeeds, the return value is NULL.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GlobalFree(
            HandleRef handle);

        #region Caret definitions

        /// <summary>
        /// The CreateCaret function creates a new shape for the system caret and assigns ownership of the caret to the specified window.
        /// </summary>
        /// <param name="hWnd">Handle to the window that owns the caret.</param>
        /// <param name="hBitmap">Handle to the bitmap that defines the caret shape.</param>
        /// <param name="nWidth">Specifies the width of the caret in logical units.</param>
        /// <param name="nHeight">Specifies the height, in logical units, of the caret.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreateCaret(
            IntPtr hWnd,
            IntPtr hBitmap,
            int nWidth,
            int nHeight);

        /// <summary>
        /// The ShowCaret function makes the caret visible on the screen at the caret's current position.
        /// </summary>
        /// <param name="hWnd">Handle to the window that owns the caret.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowCaret(
            IntPtr hWnd);

        /// <summary>
        /// The DestroyCaret function destroys the caret's current shape, frees the caret from the window, and removes the caret from the screen. 
        /// </summary>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyCaret();

        /// <summary>
        /// The SetCaretPos function moves the caret to the specified coordinates.
        /// </summary>
        /// <param name="X">Specifies the new x-coordinate of the caret.</param>
        /// <param name="Y">Specifies the new y-coordinate of the caret.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetCaretPos(
            int X,
            int Y);

        #endregion

        #region MessageBoxManager Externals

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
        /// </summary>
        /// <param name="idHook">Specifies the type of hook procedure to be installed. </param>
        /// <param name="lpfn">Pointer to the hook procedure.</param>
        /// <param name="hInstance">Handle to the DLL containing the hook procedure pointed to by the lpfn parameter.</param>
        /// <param name="threadId">Specifies the identifier of the thread with which the hook procedure is to be associated.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hInstance,
            int threadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook">Handle to the hook to be removed.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        internal static extern int UnhookWindowsHookEx(
            IntPtr idHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        /// </summary>
        /// <param name="idHook">Windows 95/98/ME: Handle to the current hook. Windows NT/XP/2003: Ignored.</param>
        /// <param name="nCode">Specifies the hook code passed to the current hook procedure.</param>
        /// <param name="wParam">Specifies the wParam value passed to the current hook procedure.</param>
        /// <param name="lParam">Specifies the lParam value passed to the current hook procedure.</param>
        /// <returns>This value is returned by the next hook procedure in the chain.</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(
            IntPtr idHook,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        /// <summary>
        /// The GetWindowTextLength function retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar). 
        /// </summary>
        /// <param name="hWnd">Handle to the window or control.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters, of the text.</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowTextLengthW", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowTextLength(
            IntPtr hWnd);

        /// <summary>
        /// The EnumChildWindows function enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function.
        /// </summary>
        /// <param name="hWndParent">Handle to the parent window whose child windows are to be enumerated.</param>
        /// <param name="lpEnumFunc">Pointer to an application-defined callback function.</param>
        /// <param name="lParam">Specifies an application-defined value to be passed to the callback function.</param>
        /// <returns>Not used.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumChildWindows(
            IntPtr hWndParent,
            EnumChildProc lpEnumFunc,
            IntPtr lParam);

        /// <summary>
        /// The GetClassName function retrieves the name of the class to which the specified window belongs.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="lpClassName">Pointer to the buffer that is to receive the class name string.</param>
        /// <param name="nMaxCount">Specifies the length, in TCHAR, of the buffer pointed to by the lpClassName parameter.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHAR copied to the specified buffer.</returns>
        [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
        internal static extern int GetClassName(
            IntPtr hWnd,
            StringBuilder lpClassName,
            int nMaxCount);

        /// <summary>
        /// The GetDlgCtrlID function retrieves the identifier of the specified control.
        /// </summary>
        /// <param name="hwndCtl">Handle to the control. </param>
        /// <returns>If the function succeeds, the return value is the identifier of the control.</returns>
        [DllImport("user32.dll")]
        internal static extern int GetDlgCtrlID(
            IntPtr hwndCtl);

        /// <summary>
        /// The GetDlgItem function retrieves a handle to a control in the specified dialog box.
        /// </summary>
        /// <param name="hDlg">Handle to the dialog box that contains the control.</param>
        /// <param name="nIDDlgItem">Specifies the identifier of the control to be retrieved.</param>
        /// <returns>If the function succeeds, the return value is the window handle of the specified control.</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDlgItem(
            IntPtr hDlg,
            int nIDDlgItem);

        /// <summary>
        /// The SetWindowText function changes the text of the specified window's title bar (if it has one).
        /// </summary>
        /// <param name="hWnd">Handle to the window or control whose text is to be changed.</param>
        /// <param name="lpString">Pointer to a null-terminated string to be used as the new title or control text.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowText(
            IntPtr hWnd,
            string lpString);

        #endregion

        /// <summary>
        /// The SetForegroundWindow function puts the thread that created the specified window into the foreground and activates the window.
        /// </summary>
        /// <param name="hWnd">Handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>If the window was brought to the foreground, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Truncates a path to fit within a certain number of characters by replacing path components with ellipses.
        /// </summary>
        /// <param name="pszOut">The address of the string that has been altered.</param>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path to be altered.</param>
        /// <param name="cchMax">The maximum number of characters to be contained in the new string, including the terminating NULL character.</param>
        /// <param name="reserved">Reserved - bit used</param>
        /// <returns>Returns TRUE if successful, or FALSE otherwise.</returns>
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool PathCompactPathEx(
            StringBuilder pszOut,
            string pszPath,
            int cchMax,
            int reserved);

        #endregion

        #region Structures

        /// <summary>
        /// Describes the properties of a file that is being copied by means of the clipboard during a Microsoft ActiveX drag-and-drop operation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct FILEDESCRIPTOR
        {
            /// <summary>
            /// An array of flags that indicate which of the other structure members contain valid data.
            /// </summary>
            public uint dwFlags;

            /// <summary>
            /// The file class identifier.
            /// </summary>
            public Guid clsid;

            /// <summary>
            /// The width and height of the file icon.
            /// </summary>
            public Size sizel;

            /// <summary>
            /// The screen coordinates of the file object.
            /// </summary>
            public Point pointl;
            
            /// <summary>
            /// File attribute flags.
            /// </summary>
            public uint dwFileAttributes;

            /// <summary>
            /// The FILETIME structure that contains the time of file creation.
            /// </summary>
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;

            /// <summary>
            /// The FILETIME structure that contains the time that the file was last accessed.
            /// </summary>
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;

            /// <summary>
            /// The FILETIME structure that contains the time of the last write operation.
            /// </summary>
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;

            /// <summary>
            /// The high-order DWORD of the file size, in bytes.
            /// </summary>
            public uint nFileSizeHigh;

            /// <summary>
            /// The low-order DWORD of the file size, in bytes.
            /// </summary>
            public uint nFileSizeLow;

            /// <summary>
            /// The null-terminated string that contains the name of the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
        }

        #region MessageBoxManager structures

        /// <summary>
        /// The CWPRETSTRUCT structure defines the message parameters passed to a WH_CALLWNDPROCRET hook procedure, CallWndRetProc.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CWPRETSTRUCT
        {
            /// <summary>
            /// Specifies the return value of the window procedure that processed the message specified by the message value.
            /// </summary>
            public IntPtr lResult;

            /// <summary>
            /// Specifies additional information about the message.
            /// </summary>
            public IntPtr lParam;

            /// <summary>
            /// Specifies additional information about the message.
            /// </summary>
            public IntPtr wParam;

            /// <summary>
            /// Specifies the message.
            /// </summary>
            public uint message;

            /// <summary>
            /// Handle to the window that processed the message specified by the message value.
            /// </summary>
            public IntPtr hwnd;
        }

        #endregion

        #endregion
    }
}
