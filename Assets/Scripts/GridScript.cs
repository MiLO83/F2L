using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
public class GridScript : MonoBehaviour
{
    public InputDevice LPMiniMK3In;
    public InputDevice MIDIIN2;
    public OutputDevice LPMiniMK3Out;
    public OutputDevice MIDIOUT2;
    public InputDevice LPMK2In;
    public OutputDevice LPMK2Out;

    private void OnEventReceivedMK3(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        Debug.Log("MK3 - Event received from " + midiDevice.Name + " at " + DateTime.Now + " = " + e.Event);
        if (e.Event.eData != null)
        {
            string dataString = "";
            foreach (byte b in e.Event.eData)
            {
                dataString += "0x" + b.ToString("X2") + ", ";
            }
            UnityEngine.Debug.Log(dataString);
        }
        if (e.Event.eNote != -1)
        {
            if (e.Event.eVelocity == 0 )
            {
                LPMK2Out.SendEvent(new NoteOnEvent(new SevenBitNumber((byte)e.Event.eNote), new SevenBitNumber((byte)e.Event.eVelocity)));
                //LPMK2Out.SendEvent(new NoteOffEvent(new SevenBitNumber((byte)e.Event.eNote), new SevenBitNumber((byte)e.Event.eVelocity)));

            } else
            {
                LPMK2Out.SendEvent(new NoteOnEvent(new SevenBitNumber((byte)e.Event.eNote), new SevenBitNumber((byte)e.Event.eVelocity)));
            }
        }
    }
    private void OnEventSentMK3(object sender, MidiEventSentEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        Debug.Log("MK3 - Event sent to " + midiDevice.Name + " at " + DateTime.Now + " = " + e.Event);
        if (e.Event.eData != null)
        {
            string dataString = "";
            foreach (byte b in e.Event.eData)
            {
                dataString += "0x" + b.ToString("X2") + ", ";
            }
            UnityEngine.Debug.Log(dataString);
        }
    }
    private void OnEventReceivedMK2(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        Debug.Log("MK2 - Event received from " + midiDevice.Name + " at " + DateTime.Now + " = " + e.Event);
        if (e.Event.eData != null)
        {
            
            List<byte> outBytes = new List<byte>();
            if (e.Event.eData[5] == 0x0A)
            {
                outBytes.Add(0x00);
                outBytes.Add(0x20);
                outBytes.Add(0x29);
                outBytes.Add(0x02);
                outBytes.Add(0x0D);
                outBytes.Add(0x03);
                
                for (int b = 6; b < e.Event.eData.Length-1; b+=2)
                {
                    outBytes.Add(0x00);
                    outBytes.Add(e.Event.eData[b]);
                    outBytes.Add(e.Event.eData[b+1]);
                }
                outBytes.Add(0xF7);
                LPMiniMK3Out.SendEvent(new NormalSysExEvent(outBytes.ToArray()));
                string dataString = "outBytes = ";
                foreach (byte b in outBytes.ToArray())
                {
                    dataString += "0x" + b.ToString("X2") + ", ";
                }
                UnityEngine.Debug.Log(dataString);
            }
            else if (e.Event.eData[5] == 0x0B)
            {
                outBytes.Add(0x00);
                outBytes.Add(0x20);
                outBytes.Add(0x29);
                outBytes.Add(0x02);
                outBytes.Add(0x0D);
                outBytes.Add(0x03);
                
                for (int b = 6; b < e.Event.eData.Length-1; b+=4)
                {
                    outBytes.Add(0x03);
                    outBytes.Add(e.Event.eData[b]);
                    outBytes.Add(e.Event.eData[b+1]);
                    outBytes.Add(e.Event.eData[b+2]);
                    outBytes.Add(e.Event.eData[b+3]);
                }

                outBytes.Add(0xF7);
                LPMiniMK3Out.SendEvent(new NormalSysExEvent(outBytes.ToArray()));
                string dataString = "outBytes = ";
                foreach (byte b in outBytes.ToArray())
                {
                    dataString += "0x" + b.ToString("X2") + ", ";
                }
                UnityEngine.Debug.Log(dataString);
            }
            
            else if (e.Event.eData[5] == 0x0E)
            {
                outBytes.Add(0x00);
                outBytes.Add(0x20);
                outBytes.Add(0x29);
                outBytes.Add(0x02);
                outBytes.Add(0x0D);
                outBytes.Add(0x03);
                for (int n = 11; n < 88; n++)
                {
                    outBytes.Add(0x00);
                    outBytes.Add((byte)n);
                    outBytes.Add(e.Event.eData[6]);

                }
                outBytes.Add(0xF7);
                LPMiniMK3Out.SendEvent(new NormalSysExEvent(outBytes.ToArray()));
            }
            //else
            {
                string dataString = "";
                foreach (byte b in e.Event.eData)
                {
                    dataString += "0x" + b.ToString("X2") + ", ";
                }
                UnityEngine.Debug.Log(dataString);
            }
        }
    }
    private void OnEventSentMK2(object sender, MidiEventSentEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        Debug.Log("MK2 - Event sent to " + midiDevice.Name + " at " + DateTime.Now + " = " + e.Event);
        if (e.Event.eData != null)
        {
            string dataString = "";
            foreach (byte b in e.Event.eData)
            {
                dataString += "0x" + b.ToString("X2") + ", ";
            }
            UnityEngine.Debug.Log(dataString);
            
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        for (int d = 0; d < OutputDevice.GetDevicesCount(); d++)
        {
            Debug.Log(OutputDevice.GetByIndex(d).Name);
        }
        StartCoroutine(Main());
    }
    public IEnumerator Main()
    {
        LPMiniMK3In = InputDevice.GetByName("LPMiniMK3 MIDI");     
        LPMiniMK3In.EventReceived += OnEventReceivedMK3;
        LPMiniMK3In.StartEventsListening();

        MIDIIN2 = InputDevice.GetByName("MIDIIN2 (LPMiniMK3 MIDI)");
        MIDIIN2.EventReceived += OnEventReceivedMK3;
        MIDIIN2.StartEventsListening();

        LPMiniMK3Out = OutputDevice.GetByName("LPMiniMK3 MIDI");
        LPMiniMK3Out.EventSent += OnEventSentMK3;

        MIDIOUT2 = OutputDevice.GetByName("MIDIOUT2 (LPMiniMK3 MIDI)");
        MIDIOUT2.EventSent += OnEventSentMK3;

        LPMK2In = InputDevice.GetByName("Launchpad MK2");
        LPMK2In.EventReceived += OnEventReceivedMK2;
        LPMK2In.StartEventsListening();

        LPMK2Out = OutputDevice.GetByName("Launchpad MK2");
        LPMK2Out.EventSent += OnEventSentMK2;

        LPMiniMK3Out.SendEvent(new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x29, 0x02, 0x0D, 0x10, 0x01, 0xF7 }));
        yield return new WaitForEndOfFrame();
        
        yield return new WaitForEndOfFrame();

        LPMiniMK3Out.SendEvent(new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x29, 0x02, 0x0D, 0x07, 0x00, (byte)16, 0x00, 0x15, 0x46, 0x32, 0x4c, 0x20, 0x2d, 0x20, 0x42, 0x79, 0x20, 0x3a, 0x20, 0x4d, 0x69, 0x4c, 0x4f, 0x38, 0x33, 0x20, 0x20, 0x20, 0x20, 0x20, 0xF7 }));
        yield return new WaitForSeconds(4f);
        LPMiniMK3Out.SendEvent(new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x29, 0x02, 0x0D, 0x0E, 0x01, 0xF7 }));

        yield return new WaitForSeconds(4f);
        LPMiniMK3Out.SendEvent(new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x29, 0x02, 0x0D, 0x07, 0xF7 }));

    }
    void OnApplicationQuit()
    {
        LPMiniMK3Out.SendEvent(new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x29, 0x02, 0x0D, 0x0E, 0x00, 0xF7 }));
        LPMiniMK3In.StopEventsListening();
        MIDIIN2.StopEventsListening();
        LPMK2In.StopEventsListening();
    }
    void Update()
    {
       
    }
}
