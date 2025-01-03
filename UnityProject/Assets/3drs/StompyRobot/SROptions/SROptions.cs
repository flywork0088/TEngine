﻿using System;
using System.ComponentModel;
using SRF.Service;
using UnityEngine;
using UnityEngine.Scripting;

public delegate void SROptionsPropertyChanged(object sender, string propertyName);

#if !DISABLE_SRDEBUGGER
[Preserve]
#endif
public partial class SROptions : INotifyPropertyChanged
{
    private static SROptions _current;

    public static SROptions Current
    {
        get { return _current; }
    }

#if !DISABLE_SRDEBUGGER
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnStartup()
    {
        _current = new SROptions(); // Need to reset options here so if we enter play-mode without a domain reload there will be the default set of options.
        SRServiceManager.GetService<SRDebugger.Internal.InternalOptionsRegistry>().AddOptionContainer(Current);
    }
#endif

    private static SRDebugger.Internal.InternalOptionsRegistry _internalOptionsRegistry;

    private static SRDebugger.Internal.InternalOptionsRegistry InternalOptionsRegistr
    {
        get
        {
            if (null == _internalOptionsRegistry)
            {
                _internalOptionsRegistry = SRServiceManager.GetService<SRDebugger.Internal.InternalOptionsRegistry>();
            }

            return _internalOptionsRegistry;
        }
    }
    
    public static void AddGameTools(object obj)
    {
        InternalOptionsRegistr.AddOptionContainer(obj);
    }
    
    public static void RemoveGameTools(Type type)
    {
        InternalOptionsRegistr.RemoveOptionContainer(type);
    }

    public event SROptionsPropertyChanged PropertyChanged;
    
#if UNITY_EDITOR
    [JetBrains.Annotations.NotifyPropertyChangedInvocator]
#endif
    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, propertyName);
        }

        if (InterfacePropertyChangedEventHandler != null)
        {
            InterfacePropertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private event PropertyChangedEventHandler InterfacePropertyChangedEventHandler;

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add { InterfacePropertyChangedEventHandler += value; }
        remove { InterfacePropertyChangedEventHandler -= value; }
    }
}
