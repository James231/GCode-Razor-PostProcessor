# G-Code Razor

[G-Code](https://en.wikipedia.org/wiki/G-code) the most common language used for programming CNC machines.

Modern features have gradually been added to G-Code over the past few decades. Early versions of G-Code did not have support for features such as iterative loops and trigonometric functions.

This Windows 10 App is intended for engineers working with older machines that do not support modern G-Code or CAD/CAM software. It gives you modern G-Code features through a postprocessor. This means you write your G-Code slightly differently (using [Razor syntax](https://en.wikipedia.org/wiki/ASP.NET_Razor)), and press a button to generate the raw G-Code which can then be run on the machine.

![G-Code Razor Screenshot](https://cdn.jam-es.com/img/gcoderazor/screenshot1.PNG)

## Download and Run

Requires Windows 10, 64-bit.

1. Go to the [Github Releases Page](https://github.com/James231/GCode-Razor-PostProcessor/releases).

2. Under the 'Assets' section, download the `.zip` file.

3. Unzip the file and save the contents somewhere. Double click on `GCodeRazor.exe` to run it.

**Note:** In the unlikely event that this repo gets some interest, I will package the app into an installer to make the installation process smoother and reduce the chance of it being blocked by anti-virus software.


## Examples

G-Code Razor includes a page full of examples. But here are a couple to demonstrate its purpose:

Firstly you can use basic trigonometric functions like this:
```gcode
N10 G00 X0.2 Y@(sin(45))
```
When you press 'Generate' in G-Code razor this produces:
```gcode
N10 G00 X0.2 Y0.707106781186547
```
Or you can add rounding with:
```gcode
N10 G00 X0.2 Y@(round(sin(45)), 4)
```
Which gives:
```gcode
N10 G00 X0.2 Y0.7071
```

The biggest time saver comes when using loops. For example, the following code requires lots of copy and pasting and editing:

```gcode
N10 G00 X0.2
N10 G00 X0.1
N10 G00 X0.3
N10 G00 X0.1
N10 G00 X0.4
N10 G00 X0.1
N10 G00 X0.5
N10 G00 X0.1
N10 G00 X0.6
```
**Note:** The line numbers are not set correctly, but this rarely causes problems, and can be set corrected your primary G-Code editor.

But, instead of typing the code above you can enter the following into G-Code Razor:

```
@for(int i = 0; i < 5; i++) {
@:N10 G00 X@(0.2+(i*0.1))
@:N10 G00 X0.1
}
```
While this may look complex, the examples page in the App explains this in detail and lets you copy snippets to change yourself. It is simpler than it looks!

## Settings

G-Code Razor has a few customizable settings. You can change them by editing the `settings.json` file. These are the settings in the version on the Releases page:

```json
{
  "open_file_when_generated": false,
  "trim_output": true,
  "files_to_concat": [
      "cshtml/trig.cshtml"
  ],
  "font_size": 20,
  "line_numbers": true,
  "word_wrap": false,
  "ask_before_closing": true
}
```

The file should consist of valid [Json](https://www.w3schools.com/js/js_json_syntax.asp).

## C# and Razor

The tool effectively gives you access to the whole of the C# programming language and .NET Framework libraries.

The full Razor documentation can be found [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1). This basically tells you how to use the `@` signs.

Functions like `sin`, `cos` and `round` are C# functions I have written. They can be edited and you can add your own. For example, `sin` is defined by:
```cs
double sin(double angle) {
    return System.Math.Sin(ToRadians(angle));
}
```
These functions are defined in the `cshtml/trig.cshtml` file. You can edit the file or add more `cshtml` files providing you list the file paths in the `files_to_concat` setting in `settings.json`.

The contents of these `cshtml` files is added to the start of the code you type into G-Code Razor (just string concatenation) before generating using [RazorLight](https://github.com/toddams/RazorLight).

Not all .NET Framework libraries are included by default (there are a lot of them) so if something you need is missing, you need to build from source after adding a reference to the solution.

## Building From Source

Use Visual Studio 2019 or later, with desktop development workloads installed. Open the solution (`.sln` file) and build.

**Note:** If you build this way the `settings.json` will be different, as the `cshtml/trig.cshtml` file will not be present. This means functions like `sin`, `cos`, `round`, etc, will be missing. The files can be copied from the version on the Releases page to fix this.

## Dependencies

[RazorLight](https://github.com/toddams/RazorLight) - Provides API around Razor engine to make it possible to use in any kind of .NET app.

[Material Design In Xaml](http://materialdesigninxaml.net/) - The WPF styles used in this app.

[AvalonEdit](http://avalonedit.net/) - The code editor WPF control used for the code fields in the app.

[AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes) - Implementation of Themes in AvalonEdit.

[SharpDevelop](https://github.com/icsharpcode/SharpDevelop) - An IDE which uses AvalonEdit. I used some of their syntax highlighting definitions. Although I heavily edited them and wrote my own G-Code highlighting.

[Custom Window Title Bar](https://github.com/James231/WPF_CustomWindow_TitleBar) - My own repo letting you customize Windows 10 title bars in WPF, based on a fork of [this](https://github.com/GiGong/WPF_CustomWindow_TitleBar).

[Json.NET](https://www.newtonsoft.com/json) - JSON serializer.

[JsonSubTypes](https://www.newtonsoft.com/json) - JSON SubType implementation for Json.NET.

Obviously credit goes to Microsoft for C#, Razor, WPF, .NET and lots of other stuff.

## License

This code is released under MIT license. Modify, distribute, sell, fork, and use this as much as you like. Both for personal and commercial use. I hold no responsibility if anything goes wrong.

If you use this, you don't need to refer to this repo, or give me any kind of credit but it would be appreciated. At least a :star: would be nice.

## Contributing

Pull Requests are welcome. But, note that by creating a pull request you are giving me permission to merge your code and release it under the MIT license mentioned above. At no point will you be able to withdraw merged code from the repository, or change the license under which it has been made available.