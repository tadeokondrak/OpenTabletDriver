﻿using System;
using System.Runtime.InteropServices;
using NativeLib.OSX.Generic;
using NativeLib.OSX.Input;

namespace NativeLib.OSX
{
    using CGEventRef = IntPtr;
    using CGDirectDisplayID = UInt32;
    using CGEventSourceRef = IntPtr;
    using CGError = Int32;

    public static class OSX
    {
        private const string Quartz = "/System/Library/Frameworks/Quartz.framework/Versions/Current/Quartz";
        private const string Foundation = "/System/Library/Frameworks/Foundation.framework/Foundation";
        private const string IOKit = "/System/Library/Frameworks/IOKit.framework/IOKit";
        
        [DllImport(Foundation)]
        public static extern void CFRelease(IntPtr handle);

        [DllImport(Quartz)]
        public extern static CGEventRef CGEventCreate(CGEventSourceRef source);

        [DllImport(Quartz)]
        public extern static CGPoint CGEventGetLocation(CGEventRef eventRef);

        [DllImport(Quartz)]
        public extern static CGEventRef CGWarpMouseCursorPosition(CGPoint newCursorPosition);

        [DllImport(Quartz)]
        public extern static CGError CGAssociateMouseAndMouseCursorPosition(int connected);

        [DllImport(Quartz)]
        public extern static CGEventRef CGEventCreateMouseEvent(CGEventSourceRef source, CGEventType mouseType,
            CGPoint mouseCursorPosition, CGMouseButton mouseButton);

        [DllImport(Quartz)]
        public extern static CGEventRef CGEventCreateKeyboardEvent(CGEventSourceRef source, CGKeyCode virtualKey, bool keyDown);

        [DllImport(Quartz)]
        public extern static CGEventRef CGEventPost(CGEventTapLocation tap, CGEventRef eventRef);

        [DllImport(Quartz)]
        public extern static CGDirectDisplayID CGMainDisplayID();

        [DllImport(Quartz)]
        public extern static CGError CGGetActiveDisplayList(uint maxDisplays,
            [In, Out] CGDirectDisplayID[] activeDisplays, out uint displayCount);

        [DllImport(Quartz)]
        public extern static CGRect CGDisplayBounds(CGDirectDisplayID displayID);
    }
}