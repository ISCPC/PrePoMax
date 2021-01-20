using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class MyListView : ListView
{   //https://stackoverflow.com/questions/2691726/how-can-i-remove-the-selection-border-on-a-listviewitem

    // Variables                                                                                                                    


    // Constructors                                                                                                                 
    public MyListView()
    {
    }


    // Overrides                                                                                                                    
    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged(e);
        Message m = Message.Create(this.Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
        this.WndProc(ref m);
    }
    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        Message m = Message.Create(this.Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
        this.WndProc(ref m);
    }
}