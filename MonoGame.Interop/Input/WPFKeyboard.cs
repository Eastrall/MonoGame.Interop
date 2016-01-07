using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

/*--------------------------------------------------------
 * WPFKeyboard.cs
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 08/11/2015 14:56:05
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Input
{
    public static class WPFKeyboard
    {
        internal static GameModule PrimaryGameModule = null;

        internal static readonly Dictionary<Key, Keys> keys = new Dictionary<Key, Keys>()
        {
            { Key.A, Keys.A },
            { Key.B, Keys.B },
            { Key.C, Keys.C },
            { Key.D, Keys.D },
            { Key.E, Keys.E },
            { Key.F, Keys.F },
            { Key.G, Keys.G },
            { Key.H, Keys.H },
            { Key.I, Keys.I },
            { Key.J, Keys.J },
            { Key.K, Keys.K },
            { Key.L, Keys.L },
            { Key.M, Keys.M },
            { Key.N, Keys.N },
            { Key.O, Keys.O },
            { Key.P, Keys.P },
            { Key.Q, Keys.Q },
            { Key.R, Keys.R },
            { Key.S, Keys.S },
            { Key.T, Keys.T },
            { Key.U, Keys.U },
            { Key.V, Keys.V },
            { Key.W, Keys.W },
            { Key.X, Keys.X },
            { Key.Y, Keys.Y },
            { Key.Z, Keys.Z },
            { Key.Add, Keys.Add },
            { Key.Apps, Keys.Apps },
            { Key.Attn, Keys.Attn },
            { Key.Back, Keys.Back },
            { Key.BrowserBack, Keys.BrowserBack },
            { Key.BrowserFavorites, Keys.BrowserFavorites },
            { Key.BrowserForward, Keys.BrowserForward },
            { Key.BrowserHome, Keys.BrowserHome },
            { Key.BrowserRefresh, Keys.BrowserRefresh },
            { Key.BrowserSearch, Keys.BrowserSearch },
            { Key.BrowserStop, Keys.BrowserStop },
            //{ Key.Cancel, Keys.Cancel },
            //{ Key.Capital, Keys.Capital },
            { Key.CapsLock, Keys.CapsLock },
            //{ Key.Clear, Keys.Clear },
            { Key.CrSel, Keys.Crsel },
            { Key.D0, Keys.D0 },
            { Key.D1, Keys.D1 },
            { Key.D2, Keys.D2 },
            { Key.D3, Keys.D3 },
            { Key.D4, Keys.D4 },
            { Key.D5, Keys.D5 },
            { Key.D6, Keys.D6 },
            { Key.D7, Keys.D7 },
            { Key.D8, Keys.D8 },
            { Key.D9, Keys.D9 },
            { Key.Decimal, Keys.Decimal },
            { Key.Delete, Keys.Delete },
            { Key.Divide, Keys.Divide },
            { Key.Down, Keys.Down },
            { Key.End, Keys.End },
            { Key.Enter, Keys.Enter },
            { Key.EraseEof, Keys.EraseEof },
            { Key.Escape, Keys.Escape },
            { Key.Execute, Keys.Execute },
            { Key.ExSel, Keys.Exsel },
            { Key.F1, Keys.F1 },
            { Key.F2, Keys.F2 },
            { Key.F3, Keys.F3 },
            { Key.F4, Keys.F4 },
            { Key.F5, Keys.F5 },
            { Key.F6, Keys.F6 },
            { Key.F7, Keys.F7 },
            { Key.F8, Keys.F8 },
            { Key.F9, Keys.F9 },
            { Key.F10, Keys.F10 },
            { Key.F11, Keys.F11 },
            { Key.F12, Keys.F12 },
            { Key.F13, Keys.F13 },
            { Key.F14, Keys.F14 },
            { Key.F15, Keys.F15 },
            { Key.F16, Keys.F16 },
            { Key.F17, Keys.F17 },
            { Key.F18, Keys.F18 },
            { Key.F19, Keys.F19 },
            { Key.F20, Keys.F20 },
            { Key.F21, Keys.F21 },
            { Key.F22, Keys.F22 },
            { Key.F23, Keys.F23 },
            { Key.F24, Keys.F24 },
            //{ Key.FinalMode, Keys.FinalMode },
            //{ Key.HangulMode, Keys.HangulMode }, // Korean
            //{ Key.HanjaMode, Keys.HanjaMode },
            { Key.Help, Keys.Help },
            { Key.Home, Keys.Home },
            //{ Key.ImeAccept, Keys.ImeAccept },
            { Key.ImeConvert, Keys.ImeConvert },
            //{ Key.ImeModeChange, Keys.ImeModeChange },
            { Key.ImeNonConvert, Keys.ImeNoConvert },
            //{ Key.ImeProcessed, Keys.ImeProcessed },
            { Key.Insert, Keys.Insert },
            { Key.LaunchApplication1, Keys.LaunchApplication1 },
            { Key.LaunchApplication2, Keys.LaunchApplication2 },
            { Key.LaunchMail, Keys.LaunchMail },
            { Key.Left, Keys.Left },
            { Key.LeftAlt, Keys.LeftAlt },
            { Key.LeftCtrl, Keys.LeftControl },
            { Key.LeftShift, Keys.LeftShift },
            { Key.LWin, Keys.LeftWindows },
            { Key.MediaNextTrack, Keys.MediaNextTrack },
            { Key.MediaPlayPause, Keys.MediaPlayPause },
            { Key.MediaPreviousTrack, Keys.MediaPreviousTrack },
            { Key.MediaStop, Keys.MediaStop },
            { Key.Multiply, Keys.Multiply },
            //{ Key.None, Keys.None },
            { Key.NumLock, Keys.NumLock },
            { Key.NumPad0, Keys.NumPad0 },
            { Key.NumPad1, Keys.NumPad1 },
            { Key.NumPad2, Keys.NumPad2 },
            { Key.NumPad3, Keys.NumPad3 },
            { Key.NumPad4, Keys.NumPad4 },
            { Key.NumPad5, Keys.NumPad5 },
            { Key.NumPad6, Keys.NumPad6 },
            { Key.NumPad7, Keys.NumPad7 },
            { Key.NumPad8, Keys.NumPad8 },
            { Key.NumPad9, Keys.NumPad9 },
            { Key.Oem8, Keys.Oem8 },
            { Key.OemAttn, Keys.OemAuto },
            { Key.OemBackslash, Keys.OemBackslash },
            //{ Key.OemBackTab, Keys.OemBackTab },
            { Key.OemClear, Keys.OemClear },
            { Key.OemCloseBrackets, Keys.OemCloseBrackets },
            { Key.OemComma, Keys.OemComma },
            { Key.OemCopy, Keys.OemCopy },
            { Key.OemEnlw, Keys.OemEnlW },
            //{ Key.OemFinish, Keys.OemFinish },
            { Key.OemMinus, Keys.OemMinus },
            { Key.OemOpenBrackets, Keys.OemOpenBrackets },
            { Key.OemPeriod, Keys.OemPeriod },
            { Key.OemPipe, Keys.OemPipe },
            { Key.OemPlus, Keys.OemPlus },
            { Key.OemQuestion, Keys.OemQuestion },
            { Key.OemQuotes, Keys.OemQuotes },
            { Key.OemSemicolon, Keys.OemSemicolon },
            { Key.OemTilde, Keys.OemTilde },
            { Key.Pa1, Keys.Pa1 },
            { Key.PageDown, Keys.PageDown },
            { Key.PageUp, Keys.PageUp },
            { Key.Pause, Keys.Pause },
            { Key.Play, Keys.Play },
            { Key.Print, Keys.Print },
            { Key.PrintScreen, Keys.PrintScreen },
            //{ Key.Prior, Keys.ProcessKey },
            //{ Key.Return, Keys.Return; },
            { Key.Right, Keys.Right },
            { Key.RightAlt, Keys.RightAlt },
            { Key.RightCtrl, Keys.RightControl },
            { Key.RightShift, Keys.RightShift },
            { Key.RWin, Keys.RightWindows },
            { Key.Scroll, Keys.Scroll },
            { Key.Select, Keys.Select },
            { Key.SelectMedia, Keys.SelectMedia },
            { Key.Separator, Keys.Separator },
            { Key.Sleep, Keys.Sleep },
            //{ Key.Snapshot, Keys.Snapshot },
            { Key.Space, Keys.Space },
            { Key.Subtract, Keys.Subtract },
            //{ Key.System, Keys.System },
            { Key.Tab, Keys.Tab },
            { Key.Up, Keys.Up },
            { Key.VolumeDown, Keys.VolumeDown },
            { Key.VolumeMute, Keys.VolumeMute },
            { Key.VolumeUp, Keys.VolumeUp },
            { Key.Zoom, Keys.Zoom }
        };

        /// <summary>
        /// Gets current keyboard state.
        /// </summary>
        /// <returns></returns>
        public static KeyboardState GetState()
        {
            return (PrimaryGameModule != null) ? PrimaryGameModule.KeyboardState : default(KeyboardState);
        }
    }
}
