//---------------------------------------------------------------------------------
//IronTextBox.cs - version 2.0.2.0b
// TextBox control based class designed to be used with Microsoft's IronPython.
// Maybe useful for testing Python scripts with IronPython. 
//WHAT'S NEW: 
//      -Updated License from GNU to Expat/MIT
//      -Tested with IronPython 2.03B
//TO DO:
//      -Fix raw_input support: "s = raw_input('--> ')"
//      -Multiple arg support for "paths" command. eg. "paths -misc -python24"
//      -Intellisense ToolTip
//
//BY DOWNLOADING AND USING, YOU AGREE TO THE FOLLOWING TERMS:
//Copyright (c) 2006-2008 by Joseph P. Socoloski III
//LICENSE
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//the MIT License, given here: <http://www.opensource.org/licenses/mit-license.php> 
//---------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
//using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;//ToolboxItem
using System.Drawing;       //ToolboxBitmap
using IronPython.Runtime;   //PythonDictionary
using IronPython.Hosting;   //PythonEngine
using IronRuby.Hosting;
using Microsoft.Scripting;  //ScriptDomainManager
using Microsoft.Scripting.Hosting;

using UIIronTextBox.IPEWrapper;

namespace UIIronTextBox
{
    #region IronTextBox Class
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(IronTextBox))]
    [DesignerAttribute(typeof(IronTextBoxControl))]
    public class IronTextBox : RichTextBox
    {
        #region IronTextBox members
        /// <summary>
        /// Default prompt text.
        /// </summary>
        private string prompt = ">>> ";

        /// <summary>
        /// Used for storing commands.
        /// </summary>
        private UIIronTextBox.CommandHistory commandHistory = new CommandHistory();

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Holds temporary defFunc lines.
        /// </summary>
        public System.Text.StringBuilder defStmtBuilder = new System.Text.StringBuilder();

        /// <summary>
        /// StringCollection of all MiscDirs
        /// </summary>
        public static StringCollection scMisc = new StringCollection();

        /// <summary>
        /// StringCollection of all Python24Dirs
        /// </summary>
        public static StringCollection scPython24 = new StringCollection();

        /// <summary>
        /// StringCollection of all IronPythonDirs
        /// </summary>
        public static StringCollection scIronPython = new StringCollection();

        /// <summary>
        /// Intellisense ToolTip.
        /// </summary>
        System.Windows.Forms.ToolTip intellisense = new System.Windows.Forms.ToolTip();

        /// <summary>
        /// True if currently processing raw_text()
        /// </summary>
        public static Boolean IsRawInput = false;

        /// <summary>
        /// Hold raw_input prompt by user
        /// </summary>
        public string rawprompt = "";

        #endregion IronTextBox members

        internal IronTextBox()
        {
            InitializeComponent();
            this.WordWrap = false;
            // Set up the delays for the ToolTip.
            intellisense.AutoPopDelay = 1000;
            intellisense.InitialDelay = 100;
            intellisense.ReshowDelay = 100;
            // Force the ToolTip text to be displayed whether or not the form is active.
            intellisense.ShowAlways = true;
        }

        #region Overrides
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Overridden to protect against deletion of contents
        /// cutting the text and deleting it from the context menu
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            // This happens before key press handler
            // Need to pass to base to have message handled
            //System.Console.WriteLine("Window message received: " + m.Msg.ToString());
            switch (m.Msg)
            {
                case 0x0302: //WM_PASTE
                case 0x0300: //WM_CUT
                case 0x000C: //WM_SETTEXT
                    break;
                case 0x0303: //WM_CLEAR
                    return;
            }
            base.WndProc(ref m);
        }
        #endregion Overrides

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // consoleTextBox
            // 
            this.Dock = DockStyle.Fill;
            this.Location = new Point(0, 0);
            this.MaxLength = 0;
            this.Multiline = true;
            this.AcceptsTab = true;
            this.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.Size = new Size(400, 176);
            this.TabIndex = 0;
            this.Text = "";
            this.KeyPress += new KeyPressEventHandler(this.consoleTextBox_KeyPress);
            this.KeyDown += new KeyEventHandler(ConsoleControl_KeyDown);
            // 
            // IronTextBoxControl
            // 
            this.Name = "IronTextBox";
            this.Size = new Size(400, 176);
            this.ResumeLayout(false);

        }
        #endregion

        #region IronTextBox Base Methods
        /// <summary>
        /// Sends the prompt to the IronTextBox
        /// </summary>
        public void printPrompt()
        {
            string currentText = this.Text;

            //add newline if it does not exist
            if ((currentText.Length != 0) && (currentText[currentText.Length - 1] != '\n'))
                printLine();

            //add the prompt
            this.AddText(Prompt);
            this.Select(this.TextLength - prompt.Length, prompt.Length - 2);
            this.SelectionColor = Color.Red;
            this.Select(this.TextLength, 0); // clears the selection
            MoveCaretToEndOfText();
            this.SelectionColor = Color.White;
        }


        /// <summary>
        /// Sends a newline character to the IronTextBox
        /// </summary>
        public void printLine()
        {
            this.AddText(System.Environment.NewLine);
        }

        /// <summary>
        /// Returns currentline's text string
        /// </summary>
        /// <returns>Returns currentline's text string</returns>
        public string GetTextAtPrompt()
        {
            if (GetCurrentLine() != "")
                return GetCurrentLine().Substring(prompt.Length);
            else
            {
                string mystring = (string)this.Lines.GetValue(this.Lines.Length - 2);
                return mystring.Substring(prompt.Length);
            }
        }

        /// <summary>
        /// Add a command to IronTextBox command history.
        /// </summary>
        /// <param name="currentCommand">IronTextBox command line</param>
        public void AddcommandHistory(string currentCommand)
        {
            commandHistory.Add(currentCommand);
        }

        /// <summary>
        /// Returns true if (char)13 '\r'
        /// </summary>
        /// <param name="keyChar">char of keypressed</param>
        /// <returns>Returns true if (char)13 '\r'</returns>
        private bool IsTerminatorKey(char keyChar)
        {
            //System.Console.WriteLine((int)keyChar);
            return ((int)keyChar) == 13;
        }

        /// <summary>
        /// Returns the current line, including prompt.
        /// </summary>
        /// <returns>Returns the current line, including prompt.</returns>
        private string GetCurrentLine()
        {
            if (this.Lines.Length > 0)
            {
                return (string)this.Lines.GetValue(this.Lines.GetLength(0) - 1);
            }
            else
                return "";
        }

        /// <summary>
        /// Replaces the text at the current prompt.
        /// </summary>
        /// <param name="text">new text to replace old text.</param>
        private void ReplaceTextAtPrompt(string text)
        {
            string currentLine = GetCurrentLine();
            int charactersAfterPrompt = currentLine.Length - prompt.Length;

            if (charactersAfterPrompt == 0)
                this.AddText(text);
            else
            {
                this.Select(this.TextLength - charactersAfterPrompt, charactersAfterPrompt);
                this.SelectedText = text;
            }
        }

        /// <summary>
        /// Returns true if caret is positioned on the currentline.
        /// </summary>
        /// <returns>Returns true if caret is positioned on the currentline.</returns>
        private bool IsCaretAtCurrentLine()
        {
            return this.TextLength - this.SelectionStart <= GetCurrentLine().Length;
        }

        /// <summary>
        /// Adds text to the IronTextBox
        /// </summary>
        /// <param name="text">text to be added</param>
        public void AddText(string text)
        {
            //Optional////////////
            // DSB scollection.Add(text);  //Optional
            //this.Text = StringCollecttostring(scollection); //Optional
            //////////////////////

            // DSB 
            this.Enabled = false;
            //System.Console.WriteLine("addText: '{0}'", text);
            this.Text += text;
            MoveCaretToEndOfText();
            this.Enabled = true;
            this.Focus();
            // DSB this.Update();
        }

        /// <summary>
        /// Returns a string retrieved from a StringCollection.
        /// </summary>
        /// <param name="inCol">StringCollection to be searched.</param>
        public string StringCollecttostring(System.Collections.Specialized.StringCollection inCol)
        {
            string value = "";
            System.Collections.Specialized.StringEnumerator myEnumerator = inCol.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                value += myEnumerator.Current;
            }

            return value;
        }

        /// <summary>
        /// Move caret to the end of the current text.
        /// </summary>
        private void MoveCaretToEndOfText()
        {
            this.SelectionStart = this.TextLength;
            this.Select(this.Text.Length, 0);
            //this.ScrollToCaret();
            //System.Console.WriteLine("scroll to caret!");
        }

        /// <summary>
        /// Returns true is the caret is just before the current prompt.
        /// </summary>
        /// <returns></returns>
        public bool IsCaretJustBeforePrompt()
        {
            return IsCaretAtCurrentLine() && GetCurrentCaretColumnPosition() == prompt.Length;
        }

        /// <summary>
        /// Returns the column position. Useful for selections.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentCaretColumnPosition()
        {
            string currentLine = GetCurrentLine();
            int currentCaretPosition = this.SelectionStart;
            //System.Console.WriteLine("pos={0}", (currentCaretPosition - this.TextLength + currentLine.Length));
            return (currentCaretPosition - this.TextLength + currentLine.Length);
        }

        /// <summary>
        /// Is the caret at a writable position.
        /// </summary>
        /// <returns></returns>

        private bool IsCaretAtWritablePosition()
        {
            return IsCaretAtCurrentLine() && GetCurrentCaretColumnPosition() > prompt.Length;
        }

        /// <summary>
        /// Sets the text of the prompt.  
        /// </summary>
        /// <param name="val">string of new prompt</param>
        public void SetPromptText(string val)
        {
            //string currentLine = GetCurrentLine();
            //this.Select(0, prompt.Length);
            //this.SelectedText = val;
            prompt = val;
        }

        /// <summary>
        /// Gets and sets the IronTextBox prompt.
        /// </summary>
        public string Prompt
        {
            get { return prompt; }
            set { SetPromptText(value); }
        }

        /// <summary>
        /// Returns the string array of the command history. 
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandHistory()
        {
            return commandHistory.GetCommandHistory();
        }

        /// <summary>
        /// Adds text to the IronTextBox.
        /// </summary>
        /// <param name="text"></param>
        public void WriteText(string text)
        {
            this.AddText(text);
        }

        #region IronTextBox Events
        /// <summary>
        /// Handle KeyPress events here.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyPressEventArgs</param>
        private void consoleTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Handle "keypress here"
            // Need to handle non-keypress in the WndProc overload
            //If current key is a backspace and is just before prompt, then stay put!
            //System.Console.WriteLine("console key PRESS KeyChar: " + (int)e.KeyChar);

            //If current key is enter
            if (IsTerminatorKey(e.KeyChar))
            {
                //**ANY CHANGES HERE MUST ALSO BE COPIED TO SimEnter()**
                e.Handled = true;
                string currentCommand = GetTextAtPrompt();

                //Optional: add the command to the stringcollection
                scollection.Add(currentCommand);
                ///////////////////////////////////////////////////

                //If it is not an empty command, then "fire" the command
                if (currentCommand.Length != 0 && this.defStmtBuilder.Length == 0 && !IsRawInput)
                {
                    if (!currentCommand.Trim().Contains("raw_input"))
                        printLine();
                    ((UIIronTextBox.IronTextBoxControl)this.Parent).FireCommandEntered(currentCommand);
                    commandHistory.Add(currentCommand);
                }

                //if we are doing a def statement (currentCommand.EndsWith(":"))
                if (this.defStmtBuilder.Length != 0)
                {
                    if (currentCommand.EndsWith(":"))
                    {
                        //we are in the first line of a def, it has already printed to console

                        //autoindent the current autoindent value
                        //int asize = Parser.GetNextAutoIndentSize(this.defStmtBuilder.ToString()+"\r\n", 4);

                        //don't printPrompt();
                        ReplaceTextAtPrompt("..." + CreateIndentstring(4));
                        e.Handled = true;
                        return;

                    }
                    else//We are past the first line, and are indenting or ending a def
                    {
                        this.defStmtBuilder.Append(currentCommand + "\r\n");

                        //if it is an empty command let's see if we just finished a def statement
                        if (currentCommand.Trim().Equals(""))
                        {
                            ((UIIronTextBox.IronTextBoxControl)this.Parent).FireCommandEntered(this.defStmtBuilder.ToString().Trim());
                            commandHistory.Add(this.defStmtBuilder.ToString());

                            //we just finished a def so clear the defbuilder
                            this.defStmtBuilder = this.defStmtBuilder.Remove(0, this.defStmtBuilder.Length);
                        }
                        else
                        {
                            //don't printPrompt();
                            AddText("\r\n..." + CreateIndentstring(4));
                            e.Handled = true;
                            return;
                        }
                    }
                }

                //raw_input support...
                if (currentCommand.Trim().Contains("raw_input("))
                {
                    IsRawInput = true;

                    //Note: if raw_input is in quotes this will not work
                    //fyi: IronPython.Modules.Builtin.RawInput();
                    //remove the "\r\n" from IPEWrapper
                    this.Text = this.Text.Remove(this.Text.Length - "\r\n".Length, "\r\n".Length);
                    rawprompt = (string)this.Lines.GetValue(this.Lines.Length - 1);
                    MoveCaretToEndOfText();

                    //AddText(temp);
                    e.Handled = true;
                    return;
                }

                if (IsRawInput)
                {
                    string rawcommand = (string)this.Lines.GetValue(this.Lines.Length - 2);
                    rawcommand = rawcommand.Replace(Prompt, "");
                    string tempprompt = (string)this.Lines.GetValue(this.Lines.Length - 1);
                    if (rawcommand.Trim().Equals("raw_input()"))
                    {
                        IsRawInput = false;
                    }
                    else// s = raw_input('--> ')
                    {
                        IsRawInput = false;
                        rawprompt = "";
                        e.Handled = true;
                        printPrompt();
                        return;
                    }
                }
                //if(GetTextAtPrompt().Trim().Equals(""))
                printPrompt();
            }
            /*
            // Handle backspace and stringcollection to help the commandhistory accuracy and debugging.
            if (e.KeyChar == (char)8 && (GetStringCollectValue(scollection, scollection.Count - 1).Length == 1) && commandHistory.LastCommand.Contains(GetStringCollectValue(scollection, scollection.Count - 1)))
            {
                scollection.RemoveAt(scollection.Count - 1);
            }*/
        }

        /// <summary>
        /// Build a string of returning spaces for indenting
        /// </summary>
        /// <param name="indentsize"></param>
        /// <returns></returns>
        public string CreateIndentstring(int indentsize)
        {
            string r = "";
            for (int i = 0; i < indentsize; i++)
            {
                r += " ";
            }
            return r;
        }

        /// <summary>
        /// KeyEvent control for staying inside the currentline and autocomplete features
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyEventArgs</param>
        private void ConsoleControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //System.Console.WriteLine("console key DOWN KeyCode: " + (int)e.KeyCode);

            if (IsCaretJustBeforePrompt())
            {
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Left)
                {
                    // eat them!
                    e.Handled = true;
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Home)
            {
                /// DSB 
                string currentLine = GetCurrentLine();
                this.SelectionStart = this.Text.Length - currentLine.Length + prompt.Length;
                //System.Console.WriteLine("currentLine = {0}, SelectionStart = {1}", currentLine, SelectionStart);
                //this.ScrollToCaret();
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (commandHistory.DoesNextCommandExist())
                {
                    ReplaceTextAtPrompt(commandHistory.GetNextCommand());
                }
                else
                    ReplaceTextAtPrompt("");
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (commandHistory.DoesPreviousCommandExist())
                {
                    ReplaceTextAtPrompt(commandHistory.GetPreviousCommand());
                }
                else
                    ReplaceTextAtPrompt(""); // FIXME: use templine, if started, and then ""
                e.Handled = true;
            }
            /*
        else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
        {
            // Performs command completion
            string currentTextAtPrompt = GetTextAtPrompt();
            string lastCommand = commandHistory.LastCommand;

            //If the last command is not nul and no text at the current prompt or lastcommand starts with the currenttext at the current prompt,
            //then autofill because the right arrow key was pressed.
            if (lastCommand != null && (currentTextAtPrompt.Length == 0 || lastCommand.StartsWith(currentTextAtPrompt)))
            {
                if (lastCommand.Length > currentTextAtPrompt.Length)
                {
                    int i = scollection.Count;
                    //scollection.Insert(scollection.Count, lastCommand[currentTextAtPrompt.Length].ToString());
                    this.AddText(lastCommand[currentTextAtPrompt.Length].ToString());
                }
            }
        }
             */
        }


        #endregion IronTextBox Events

        #endregion IronTextBox Base Methods

        #region IronTextBox IronPython Support
        /// <summary>
        /// Stores input commands from IronTextBox
        /// </summary>
        StringCollection input = new StringCollection();

        /// <summary>
        /// Stores output generated from IronPython
        /// </summary>
        StringCollection output = new StringCollection();

        #endregion IronTextBox IronPython Support

        #region StringCollection support
        /// <summary>
        /// Commands and strings from IronTextBox.AddText() gets stored here
        /// Status: Currently not used 3/12/06 11:16am
        /// </summary>
        System.Collections.Specialized.StringCollection scollection = new System.Collections.Specialized.StringCollection();

        /// <summary>
        /// Returns a string retrieved from a StringCollection.
        /// </summary>
        /// <param name="inCol">StringCollection to be searched.</param>
        /// <param name="index">index of StringCollection to retrieve.</param>
        public string GetStringCollectValue(System.Collections.Specialized.StringCollection inCol, int index)
        {
            string value = "";
            int count = 0;
            System.Collections.Specialized.StringEnumerator myEnumerator = inCol.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                if (index == count)
                {
                    value = myEnumerator.Current;
                }

                count = count + 1;
            }

            return value;
        }
        #endregion StringCollection support
    }
    #endregion IronTextBox Class

    #region IronTextBoxControl Class
    /// <summary>
    /// Summary description for IronTextBoxControl.
    /// </summary>
    public class IronTextBoxControl : UserControl
    {
        public ScriptRuntime environment;
        public ScriptEngine engine;
        public ScriptScope scope;
        public IronTextBox consoleTextBox;
        public event EventCommandEntered CommandEntered;
        private Container components = null;
        public StringBuilder defBuilder
        {
            get { return consoleTextBox.defStmtBuilder; }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.defStmtBuilder = value;
            }
        }

        /// <summary>
        /// Returns the string array of the command history.
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandHistory()
        {
            return consoleTextBox.GetCommandHistory();
        }

        /// <summary>
        /// Gets and sets console text ForeColor. 
        /// </summary>
        public Color ConsoleTextForeColor
        {
            get { return consoleTextBox != null ? consoleTextBox.ForeColor : Color.White; }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.ForeColor = value;
            }
        }

        /// <summary>
        /// Gets and sets console text BackColor. 
        /// </summary>
        public Color ConsoleTextBackColor
        {
            get { return consoleTextBox != null ? consoleTextBox.BackColor : Color.Black; }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.BackColor = value;
            }
        }

        /// <summary>
        /// Gets and sets console Font. 
        /// </summary>
        public Font ConsoleTextFont
        {
            get { return consoleTextBox != null ? consoleTextBox.Font : new Font("Lucida Console", 8); }
            set
            {
                if (consoleTextBox != null)
                    consoleTextBox.Font = value;
            }
        }

        /// <summary>
        /// Gets and sets string to be used for the Prompt.
        /// </summary>
        public string Prompt
        {
            get { return consoleTextBox.Prompt; }
            set { consoleTextBox.Prompt = value; }
        }
    #endregion IronTextBoxControl members


        /// <summary>
        /// IronTextBoxControl
        /// </summary>

        public IronTextBoxControl()
        {
            InitializeComponent();
            ScriptRuntimeSetup scriptRuntimeSetup = new ScriptRuntimeSetup();
            scriptRuntimeSetup.LanguageSetups.Add(
                new LanguageSetup("IronPython.Runtime.PythonContext, IronPython",
                    "IronPython",
                    new[] { "IronPython", "Python", "py" },
                    new[] { ".py" }));
            scriptRuntimeSetup.LanguageSetups.Add(
                new LanguageSetup("IronRuby.Runtime.RubyContext, IronRuby",
                    "IronRupy",
                    new[] { "IronRuby", "Ruby", "rb" },
                    new[] { ".rb" }));
            environment = new ScriptRuntime(scriptRuntimeSetup);
            //environment = ScriptRuntime.CreateFromConfiguration();
            scope = environment.CreateScope();
            engine = environment.GetEngine("py");
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                engine.SetSearchPaths(new string[] {
                    "/home/dblank/Myro/Pyjama/python",
                    "/usr/lib/python2.5",
                    "/usr/lib/python2.5/site-packages"});
            }
            else
            {
                engine.SetSearchPaths(new string[] {
                    Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) 
                        + @"\Myro\Pyjama\python",
                    @"C:\Python24\Lib",
                    @"C:\Python24\site-packages",
                    @"C:\Python25\Lib",
                    @"C:\Python25\site-packages"});
            }
            // Load mscorlib.dll:
            engine.Runtime.LoadAssembly(typeof(string).Assembly);
            // Load Languages so that Host System can find DLLs:
            engine.Runtime.LoadAssembly(typeof(IronPython.Hosting.Python).Assembly);
            engine.Runtime.LoadAssembly(typeof(IronRuby.Hosting.RubyCommandLine).Assembly);
            engine.Runtime.LoadAssembly(typeof(IronRuby.StandardLibrary.BigDecimal.Fraction).Assembly);
            //Load System.dll
            engine.Runtime.LoadAssembly(typeof(System.Diagnostics.Debug).Assembly);
            //IronTextBox's CommandEntered event
            CommandEntered += new UIIronTextBox.EventCommandEntered(irontextboxControl_CommandEntered);
        }

        /// <summary>
        /// Executes the Python file within the IronTextBox environment.
        /// A nice way to quickly get a Python module in CLI to test or use.
        /// </summary>
        /// <param name="pyfile">Python file (.py)</param>
        /// <returns>object</returns>

        public object DoIPExecuteFile(string filename)
        {
            ScriptSource source = engine.CreateScriptSourceFromFile(filename);
            return source.Execute(scope);
        }

        /// <summary>
        /// Executes the code in SourceCodeKind.SingleStatement to fire the command event
        /// Use DoIPEvaluate if you do not wish to fire the command event
        /// </summary>
        /// <param name="pycode">python statement</param>
        /// <returns>object</returns>

        public object DoIPExecute(string code)
        {
            ScriptSource source = engine.CreateScriptSourceFromString(code,
                         SourceCodeKind.SingleStatement);
            return source.Execute(scope);
        }

        /// <summary>
        /// Executes the code in SourceCodeKind.Expression not to fire the command event
        /// Use DoIPExecute if you do wish to fire the command event
        /// </summary>
        /// <param name="pycode">Python expression</param>
        /// <returns>object</returns>

        public object DoIPEvaluate(string code)
        {
            ScriptSource source = engine.CreateScriptSourceFromString(code,
                         SourceCodeKind.Expression);
            return source.Execute(scope);
        }

        void irontextboxControl_CommandEntered(object sender, UIIronTextBox.CommandEnteredEventArgs e)
        {
            string command = e.Command.TrimEnd();
            DoCommand(command);
        }

        public void DoCommand(string command)
        {
            engine.Runtime.IO.SetOutput(new UIIronTextBox.IPEWrapper.IPEStreamWrapper(
                          IPEWrapper.IPEStreamWrapper.IPEngineResponse),
                    engine.Runtime.IO.InputEncoding);

            //Create a temp object to use
            object tempobject;

            //Begin IronTextBox evaluation if something there....
            if (command != "")
            {
                if (command == "clear")
                {
                    this.Clear();
                }
                else if (command == "help")
                {
                    this.WriteText(GetHelpText());
                }
                else if (command == "python")
                {
                    engine = environment.GetEngine("py");
                    Prompt = "python> ";
                }
                else if (command == "ruby")
                {
                    engine = environment.GetEngine("rb");
                    Prompt = "ruby  > ";
                }
                else if (command == "runfile")
                {
                    //runfile - Run a .Py file.  Calls OpenFileDialog to PythonEngine.RunFile....
                    //  goodfor debuging .y file within IDE
                    this.Runfile();
                }
                else if (command.TrimEnd().EndsWith(":") == true)
                {
                    //Need to do a ReadStatement...
                    try
                    {
                        bool isMultiLine = false;
                        int autoIndentSize = 0;
                        int numberOfBlankLines = 0;
                        object ExecWrapper = null;
                        bool result;

                        string line = command;
                        if (line == null)
                        {
                            if (ExecWrapper != null)
                            {
                                //Ops.Call(ExecWrapper, new object[] { null });//not needed for IP2?
                            }
                            result = false;
                        }

                        defBuilder.Append(line);
                        defBuilder.Append("\r\n");

                        bool endOfInput = (line.Length == 0);
                        bool parsingMultiLineString;
                        bool parsingMultiLineCmpdStmt;

                        //old//s = ParsetheText(consoleTextBox.global_eng.Sys, new CompilerContext(), defBuilder.ToString(), endOfInput, out parsingMultiLineString, out isMultiLine);
                        string[] seperators = new string[] { "\r" };
                        string[] allpieces = defBuilder.ToString().Split(seperators, StringSplitOptions.None);

                        if (/*Options.AutoIndentSize != 0 &&*/ line.Trim().Length == 0)
                            numberOfBlankLines++;
                        else
                            numberOfBlankLines = 0;


                        if (allpieces.Length > 1)
                        {
                            // Note that splitting a string literal over multiple lines does not 
                            // constitute a multi-line statement.
                            isMultiLine = true;
                        }

                        //autoIndentSize = Parser.GetNextAutoIndentSize(defBuilder.ToString(), autoIndentSize);//Not needed in IP2?
                        result = true;
                    }
                    catch
                    {
                    }
                }

                else //misc commands...
                {
                    bool error = false;
                    string err_message = null;
                    object result = null;
                    try
                    {
                        // FIXME: runs all at once, then prints result
                        result = DoIPExecute(command);
                    }
                    catch (Exception err)//catch any errors
                    {
                        err_message = err.Message;
                        error = true;
                    }
                    // DSB remove newline:
                    this.WriteText(IPEStreamWrapper.sbOutput.ToString());
                    if (result != null)
                    {
                        this.WriteText("--> " + result.ToString());
                    }

                    //added to fix "rearviewmirror" (IPEStreamWrapper.sbOutput not clearing) bug.
                    IPEStreamWrapper.sbOutput.Remove(0, IPEStreamWrapper.sbOutput.Length);        //Clear
                    if (error)
                    {
                        this.WriteText("\r\nPython error: " + err_message);
                    }
                }
            }
        }

        /// <summary>
        /// Displays information about IronTextBox and user's IronPython version.
        /// </summary>
        /// <returns>Returns string information about IronTextBox and user's IronPython version.</returns>
        public string GetHelpText()
        {
            string helpText;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("*********************");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("**   Pyjama Help   **");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("*********************");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("DLR version " + engine.LanguageVersion);
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append("Commands Available:");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(" clear - Clears the screen");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(" python - Switch to Python");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(" ruby - Switch to Ruby");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(" runfile - Run a .Py file.  Calls OpenFileDialog to PythonEngine.RunFile.");
            stringBuilder.Append(System.Environment.NewLine);
            helpText = stringBuilder.ToString();
            return helpText;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.consoleTextBox = new UIIronTextBox.IronTextBox();
            this.SuspendLayout();
            // 
            // consoleTextBox
            // 
            //    this.consoleTextBox.AcceptsReturn = true;
            this.consoleTextBox.AcceptsTab = true;
            this.consoleTextBox.BackColor = Color.Black;
            this.consoleTextBox.ForeColor = Color.White;
            this.consoleTextBox.Dock = DockStyle.Fill;
            this.consoleTextBox.Location = new Point(0, 0);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Prompt = "python> ";
            this.consoleTextBox.ScrollBars = RichTextBoxScrollBars.Both;
            this.consoleTextBox.Font = new Font("Lucida Console", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            this.consoleTextBox.Size = new Size(232, 216);
            this.consoleTextBox.TabIndex = 0;
            // 
            // IronTextBoxControl
            // 
            this.Controls.Add(this.consoleTextBox);
            this.Name = "IronTextBoxControl";
            this.Size = new Size(232, 216);
            this.ResumeLayout(false);
            this.consoleTextBox.AddText("Pyjama Shell\n------------\n\n");
            this.consoleTextBox.printPrompt();
        }
        #endregion

        #region Overides
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion Overides

        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="command">Command line string.</param>
        internal void FireCommandEntered(string command)
        {
            OnCommandEntered(command);
        }

        /// <summary>
        /// Creates new EventCommandEntered event.
        /// </summary>
        /// <param name="command">Command line string.</param>
        protected virtual void OnCommandEntered(string command)
        {
            if (CommandEntered != null)
                CommandEntered(command, new CommandEnteredEventArgs(command));
        }

        /// <summary>
        /// Clear the current text in the IronTextBox.
        /// </summary>
        public void Clear()
        {
            consoleTextBox.Clear();
        }

        /// <summary>
        /// Send text to the IronTextBox.
        /// </summary>
        /// <param name="text"></param>
        public void WriteText(string text)
        {
            consoleTextBox.WriteText(text);
        }

        /// <summary>
        /// Simulate the Enter KeyPress event.
        /// </summary>
        public void SimEnter()
        {
            string currentCommand = consoleTextBox.GetTextAtPrompt();
            consoleTextBox.Focus();

            //Optional: add the command to the stringcollection
            //consoleTextBox.scollection.Add(currentCommand);
            ///////////////////////////////////////////////////

            //If it is not an empty command, then "fire" the command
            if (currentCommand.Length != 0)
            {
                //consoleTextBox.printLine();
                ((UIIronTextBox.IronTextBoxControl)consoleTextBox.Parent).FireCommandEntered(currentCommand);
                consoleTextBox.AddcommandHistory(currentCommand);
            }
            else
            {
                //if it is an empty command let's see if we just finished a def statement
                if (consoleTextBox.defStmtBuilder.Length != 0)
                {
                    ((UIIronTextBox.IronTextBoxControl)consoleTextBox.Parent).FireCommandEntered(consoleTextBox.defStmtBuilder.ToString());
                    consoleTextBox.AddcommandHistory(consoleTextBox.defStmtBuilder.ToString());

                    //we just finished a def so clear the defbuilder
                    consoleTextBox.defStmtBuilder = consoleTextBox.defStmtBuilder.Remove(0, consoleTextBox.defStmtBuilder.Length);
                }
            }
            consoleTextBox.printPrompt();
        }

        /// <summary>
        /// Run a .Py file.  Calls OpenFileDialog to PythonEngine.RunFile.
        /// </summary>
        public void Runfile()
        {
            try
            {
                //Browse to the file...
                OpenFileDialog ofd = new OpenFileDialog();
                //ofd.InitialDirectory = UIIronTextBox.Paths.MiscDirs.vs_Projects;
                ofd.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";
                ofd.ShowDialog();

                DoIPExecuteFile(ofd.FileName);

            }
            catch (Exception ex)
            {
                consoleTextBox.WriteText("The file could not be read:");
                consoleTextBox.WriteText(ex.Message);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if line begins with #, or begins with """ and endwith """</returns>
        public bool IsSingleCommentLine(string line)
        {
            //Trim the end of the line because sometimes whitespace after """
            line = line.TrimEnd();

            if (line.StartsWith("#") || line.StartsWith("    #") || line.StartsWith("        #") || line.StartsWith("            #") && line != "")
            {
                return true;
            }
            else if (line.StartsWith("\"\"\"") || line.StartsWith("    \"\"\"") || line.StartsWith("        \"\"\"") || line.StartsWith("            \"\"\""))
            {
                if (line.TrimEnd().EndsWith("\"\"\"") && line.IndexOf("\"\"\"") != line.Length - 3)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns aa ArrayList from a StringCollection  
        /// </summary>
        /// <param name="StringColin">Incoming StringCollection.</param>
        public ArrayList Convert_StringCollectiontoArrayList(StringCollection StringColin)
        {
            ArrayList newArrayList = new ArrayList();

            StringEnumerator myEnumerator = StringColin.GetEnumerator();
            while (myEnumerator.MoveNext())
                newArrayList.Add(myEnumerator.Current.ToString());

            return newArrayList;
        }
    }

    #region CommandHistory Class
    internal class CommandHistory
    {
        private int currentPosn;
        private string lastCommand;
        private ArrayList commandHistory = new ArrayList();

        internal CommandHistory()
        {
        }

        internal void Add(string command)
        {
            if (command != lastCommand)
            {
                commandHistory.Add(command);
                lastCommand = command;
                currentPosn = commandHistory.Count;
            }
        }

        internal bool DoesPreviousCommandExist()
        {
            return currentPosn > 0;
        }

        internal bool DoesNextCommandExist()
        {
            return currentPosn < commandHistory.Count - 1;
        }

        internal string GetPreviousCommand()
        {
            lastCommand = (string)commandHistory[--currentPosn];
            return lastCommand;
        }

        internal string GetNextCommand()
        {
            lastCommand = (string)commandHistory[++currentPosn];
            return LastCommand;
        }

        internal string LastCommand
        {
            get { return lastCommand; }
        }

        internal string[] GetCommandHistory()
        {
            return (string[])commandHistory.ToArray(typeof(string));
        }
    }
    #endregion CommandHistory Class

    #region CommandEnteredEventArgs Class
    /// <summary>
    /// Command argument class.
    /// </summary>
    public class CommandEnteredEventArgs : EventArgs
    {
        string command;
        public CommandEnteredEventArgs(string command)
        {
            this.command = command;
        }

        public string Command
        {
            get { return command; }
        }
    }
    #endregion CommandEnteredEventArgs Class

    public delegate void EventCommandEntered(object sender, CommandEnteredEventArgs e);
}
