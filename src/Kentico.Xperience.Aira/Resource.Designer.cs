﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kentico.Xperience.Aira {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kentico.Xperience.Aira.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi, I&apos;m AIRA your AI powered coworker. I can answer questions about your Kentico Xperience data and even carry out tasks for you. Let me show you how I work.....
        /// </summary>
        internal static string InitialAiraMessageIntroduction {
            get {
                return ResourceManager.GetString("InitialAiraMessageIntroduction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Every time you open the chat dialog you can ask me directly or you can use some pre-made requests called prompts.
        /// </summary>
        internal static string InitialAiraMessagePromptExplanation {
            get {
                return ResourceManager.GetString("InitialAiraMessagePromptExplanation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is a PREVIEW version. Expect updates and improvements..
        /// </summary>
        internal static string NavigationMenuMessage {
            get {
                return ResourceManager.GetString("NavigationMenuMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please try again later, we&apos;ll be back shortly!.
        /// </summary>
        internal static string ServicePageChatTryAgainLater {
            get {
                return ResourceManager.GetString("ServicePageChatTryAgainLater", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sorry, Aira chat is currently unavailable due to maintenance..
        /// </summary>
        internal static string ServicePageChatUnavailable {
            get {
                return ResourceManager.GetString("ServicePageChatUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Images created as content items (in draft), added to your content hub.
        /// </summary>
        internal static string SmartUploadFilesUploadedMessage {
            get {
                return ResourceManager.GetString("SmartUploadFilesUploadedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT FILES.
        /// </summary>
        internal static string SmartUploadSelectFilesButton {
            get {
                return ResourceManager.GetString("SmartUploadSelectFilesButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What can I help you with ?.
        /// </summary>
        internal static string WelcomeBackAiraMessage {
            get {
                return ResourceManager.GetString("WelcomeBackAiraMessage", resourceCulture);
            }
        }
    }
}
