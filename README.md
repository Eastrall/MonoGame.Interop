MonoGame.Interop 1.0
====

MonoGame.Interop is a small library who enables you to use MonoGame into a WPF window.

Current status
----

MonoGame.Interop handles:

- Rendering (2D and 3D)
- Mouse input
- Keyboard input

To do list
----

- Component system (similar as MonoGame's system)
- General improvements

How to use
----

Tutorial incoming.

Remarks
----

Since the GameModule inherits from a ContentControl, it seems the window has some troubles disposing it and calling the ```UnloadContent()``` method to unload the game content.
To avoid some error, I recommend calling the ```Dispose()``` method of the GameModule in the hosting window on closing event like this:

```
        protected override void OnClosed(EventArgs e)
        {
            this.MAIN_GAME.Dispose();
            base.OnClosed(e);
        }
		```


Credits
----

Based on http://blogs.msdn.com/b/nicgrave/archive/2011/03/25/wpf-hosting-for-xna-game-studio-4-0.aspx 's code and other codes found on the Internet.

Thank you to tgjones for the DrawingSurface control (https://github.com/tgjones/gemini/blob/master/src/Gemini.Modules.MonoGame/Controls/DrawingSurface.cs)

Screens
----

![alt tag](https://github.com/ShyroFR/MonoGame.Interop/blob/master/Screens/Interop.PNG)
