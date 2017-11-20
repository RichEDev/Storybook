﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.1
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.1.
'
Namespace FWEmailServer
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="FWEmailServerSoap", [Namespace]:="http://tempuri.org/Framework/FWEmailServer")>  _
    Partial Public Class FWEmailServer
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private RequestAuthTokenOperationCompleted As System.Threading.SendOrPostCallback
        
        Private DoAuditCleardownOperationCompleted As System.Threading.SendOrPostCallback
        
        Private DoReviewEmailOperationCompleted As System.Threading.SendOrPostCallback
        
        Private SendEmailOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/xmas2010/framework/FWEmailServer.asmx"
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event RequestAuthTokenCompleted As RequestAuthTokenCompletedEventHandler
        
        '''<remarks/>
        Public Event DoAuditCleardownCompleted As DoAuditCleardownCompletedEventHandler
        
        '''<remarks/>
        Public Event DoReviewEmailCompleted As DoReviewEmailCompletedEventHandler
        
        '''<remarks/>
        Public Event SendEmailCompleted As SendEmailCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Framework/FWEmailServer/RequestAuthToken", RequestNamespace:="http://tempuri.org/Framework/FWEmailServer", ResponseNamespace:="http://tempuri.org/Framework/FWEmailServer", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RequestAuthToken(ByVal UserName As String, ByVal Password As String) As String
            Dim results() As Object = Me.Invoke("RequestAuthToken", New Object() {UserName, Password})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Function BeginRequestAuthToken(ByVal UserName As String, ByVal Password As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RequestAuthToken", New Object() {UserName, Password}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndRequestAuthToken(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub RequestAuthTokenAsync(ByVal UserName As String, ByVal Password As String)
            Me.RequestAuthTokenAsync(UserName, Password, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub RequestAuthTokenAsync(ByVal UserName As String, ByVal Password As String, ByVal userState As Object)
            If (Me.RequestAuthTokenOperationCompleted Is Nothing) Then
                Me.RequestAuthTokenOperationCompleted = AddressOf Me.OnRequestAuthTokenOperationCompleted
            End If
            Me.InvokeAsync("RequestAuthToken", New Object() {UserName, Password}, Me.RequestAuthTokenOperationCompleted, userState)
        End Sub
        
        Private Sub OnRequestAuthTokenOperationCompleted(ByVal arg As Object)
            If (Not (Me.RequestAuthTokenCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent RequestAuthTokenCompleted(Me, New RequestAuthTokenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Framework/FWEmailServer/DoAuditCleardown", RequestNamespace:="http://tempuri.org/Framework/FWEmailServer", ResponseNamespace:="http://tempuri.org/Framework/FWEmailServer", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub DoAuditCleardown()
            Me.Invoke("DoAuditCleardown", New Object(-1) {})
        End Sub
        
        '''<remarks/>
        Public Function BeginDoAuditCleardown(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoAuditCleardown", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Sub EndDoAuditCleardown(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub DoAuditCleardownAsync()
            Me.DoAuditCleardownAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub DoAuditCleardownAsync(ByVal userState As Object)
            If (Me.DoAuditCleardownOperationCompleted Is Nothing) Then
                Me.DoAuditCleardownOperationCompleted = AddressOf Me.OnDoAuditCleardownOperationCompleted
            End If
            Me.InvokeAsync("DoAuditCleardown", New Object(-1) {}, Me.DoAuditCleardownOperationCompleted, userState)
        End Sub
        
        Private Sub OnDoAuditCleardownOperationCompleted(ByVal arg As Object)
            If (Not (Me.DoAuditCleardownCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent DoAuditCleardownCompleted(Me, New System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Framework/FWEmailServer/DoReviewEmail", RequestNamespace:="http://tempuri.org/Framework/FWEmailServer", ResponseNamespace:="http://tempuri.org/Framework/FWEmailServer", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function DoReviewEmail(ByVal runDate As Date, ByVal daysahead As Integer) As Boolean
            Dim results() As Object = Me.Invoke("DoReviewEmail", New Object() {runDate, daysahead})
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Function BeginDoReviewEmail(ByVal runDate As Date, ByVal daysahead As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoReviewEmail", New Object() {runDate, daysahead}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndDoReviewEmail(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Overloads Sub DoReviewEmailAsync(ByVal runDate As Date, ByVal daysahead As Integer)
            Me.DoReviewEmailAsync(runDate, daysahead, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub DoReviewEmailAsync(ByVal runDate As Date, ByVal daysahead As Integer, ByVal userState As Object)
            If (Me.DoReviewEmailOperationCompleted Is Nothing) Then
                Me.DoReviewEmailOperationCompleted = AddressOf Me.OnDoReviewEmailOperationCompleted
            End If
            Me.InvokeAsync("DoReviewEmail", New Object() {runDate, daysahead}, Me.DoReviewEmailOperationCompleted, userState)
        End Sub
        
        Private Sub OnDoReviewEmailOperationCompleted(ByVal arg As Object)
            If (Not (Me.DoReviewEmailCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent DoReviewEmailCompleted(Me, New DoReviewEmailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Framework/FWEmailServer/SendEmail", RequestNamespace:="http://tempuri.org/Framework/FWEmailServer", ResponseNamespace:="http://tempuri.org/Framework/FWEmailServer", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function SendEmail(ByVal EmailType As String, ByVal toAddr As String, ByVal fromAddr As String, ByVal Subject As String, ByVal Body As String) As Boolean
            Dim results() As Object = Me.Invoke("SendEmail", New Object() {EmailType, toAddr, fromAddr, Subject, Body})
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Function BeginSendEmail(ByVal EmailType As String, ByVal toAddr As String, ByVal fromAddr As String, ByVal Subject As String, ByVal Body As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("SendEmail", New Object() {EmailType, toAddr, fromAddr, Subject, Body}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndSendEmail(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Overloads Sub SendEmailAsync(ByVal EmailType As String, ByVal toAddr As String, ByVal fromAddr As String, ByVal Subject As String, ByVal Body As String)
            Me.SendEmailAsync(EmailType, toAddr, fromAddr, Subject, Body, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub SendEmailAsync(ByVal EmailType As String, ByVal toAddr As String, ByVal fromAddr As String, ByVal Subject As String, ByVal Body As String, ByVal userState As Object)
            If (Me.SendEmailOperationCompleted Is Nothing) Then
                Me.SendEmailOperationCompleted = AddressOf Me.OnSendEmailOperationCompleted
            End If
            Me.InvokeAsync("SendEmail", New Object() {EmailType, toAddr, fromAddr, Subject, Body}, Me.SendEmailOperationCompleted, userState)
        End Sub
        
        Private Sub OnSendEmailOperationCompleted(ByVal arg As Object)
            If (Not (Me.SendEmailCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent SendEmailCompleted(Me, New SendEmailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub RequestAuthTokenCompletedEventHandler(ByVal sender As Object, ByVal e As RequestAuthTokenCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class RequestAuthTokenCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub DoAuditCleardownCompletedEventHandler(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub DoReviewEmailCompletedEventHandler(ByVal sender As Object, ByVal e As DoReviewEmailCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class DoReviewEmailCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Boolean
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Boolean)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub SendEmailCompletedEventHandler(ByVal sender As Object, ByVal e As SendEmailCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class SendEmailCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Boolean
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Boolean)
            End Get
        End Property
    End Class
End Namespace
