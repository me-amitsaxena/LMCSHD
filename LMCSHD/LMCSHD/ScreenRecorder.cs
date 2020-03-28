﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

//using Xceed.Wpf.Toolkit;

namespace LMCSHD
{
    public static class ScreenRecorder
    {
        public static Rectangle CaptureRect;
        public static bool doCapture, doOutline;

        public delegate void Callback(Bitmap capturedFrame);
        public static void StartRecording(Callback pixelDataCallback)
        {
            doCapture = true;
            while (doCapture)
                pixelDataCallback(ScreenToBitmap());
        }
        private static Bitmap ScreenToBitmap()
        {
            IntPtr handle = IntPtr.Zero;
            IntPtr hdcSrc = GetDC(handle);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, CaptureRect.Width, CaptureRect.Height);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, CaptureRect.Width, CaptureRect.Height, hdcSrc, CaptureRect.X, CaptureRect.Y, CopyPixelOperation.SourceCopy);

            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(handle, hdcSrc);
            Bitmap bitmap = Bitmap.FromHbitmap(hBitmap);

            DeleteObject(hBitmap);

            return bitmap;
        }

        public static void ShowOutline()
        {
            doOutline = true;
            while (doOutline)
            {
                IntPtr ptr = GetDC(IntPtr.Zero);
                using (Graphics g = Graphics.FromHdc(ptr))
                    g.DrawRectangle(new Pen(Color.DarkMagenta, 1), new Rectangle(CaptureRect.X - 1, CaptureRect.Y - 1, CaptureRect.Width + 2, CaptureRect.Height + 2));
                ReleaseDC(IntPtr.Zero, ptr);

                Thread.Sleep(6);
            }
        }
        public static void HideOutline()
        {
            InvalidateRect(IntPtr.Zero, IntPtr.Zero, false);
        }

        #region DLL Imports
        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
        [DllImport("user32.dll")]
        static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        #endregion
    }
}
