﻿#pragma checksum "..\..\EditTape.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BA1691F57B56FFA50FD111BF8B1D0A017A5DA9A6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DumpApplic;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DumpApplic {
    
    
    /// <summary>
    /// EditTape
    /// </summary>
    public partial class EditTape : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\EditTape.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox barCodeBox;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\EditTape.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox SystemValuesCB;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\EditTape.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox BTypeValuesCB;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\EditTape.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox detailsBox;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\EditTape.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button updateTape;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\EditTape.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancelAddTape;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DumpApplic;component/edittape.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\EditTape.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.barCodeBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.SystemValuesCB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.BTypeValuesCB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.detailsBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.updateTape = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\EditTape.xaml"
            this.updateTape.Click += new System.Windows.RoutedEventHandler(this.editTape_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cancelAddTape = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\EditTape.xaml"
            this.cancelAddTape.Click += new System.Windows.RoutedEventHandler(this.CancelEditTape_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

