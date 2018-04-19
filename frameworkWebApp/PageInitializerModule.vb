Imports Microsoft.Web.Infrastructure.DynamicModuleHelper

Public NotInheritable Class PageInitializerModule
    Implements IHttpModule
        
    Private ReadOnly _avoidedPages() As String = New String(){"/shared/logon.aspx"}
        
    Public Shared Sub Initialize()
        DynamicModuleUtility.RegisterModule(GetType(PageInitializerModule))
    End Sub
        
    Sub Init(context As HttpApplication) Implements IHttpModule.Init
        
        AddHandler context.PreRequestHandlerExecute, New EventHandler(Sub(sender As Object, e As EventArgs)
            Dim handler As IHttpHandler = context.Context.CurrentHandler
            If (handler IsNot Nothing) _
            AndAlso (Me._avoidedPages.Contains(HttpContext.Current.Request.FilePath) = false) Then
                Dim name As String = handler.[GetType]().Assembly.FullName
                If (Not name.StartsWith("System.Web") AndAlso Not name.StartsWith("Microsoft")) Then
                    [Global].InitializeHandler(handler)
                End If
            End If
        End Sub)

    End Sub

    Sub Dispose() Implements IHttpModule.Dispose
            
    End Sub
End Class