Imports System.Xml.Serialization
Imports System.Collections
Imports System.IO

Public Module modTemplateEditor
    Public iSettings As New sSettings

    Public Structure sSettings
        Dim txtShading As Boolean
        Dim txtFocus As Boolean
        Dim strLastFile As String
        Dim txtLoad As Boolean
        Dim settSave As Boolean
        Dim fileSave As Boolean
    End Structure

    Public Sub WriteSettings(ByVal DatIn As sSettings)
        Dim XSS As New XmlSerializer(GetType(sSettings))
        Dim Writer As New StreamWriter(Application.StartupPath & "\data\settings.xml", False)

        XSS.Serialize(Writer, DatIn)
        Writer.Close()
    End Sub

    Public Function ReadSettings() As sSettings
        Try
            Dim DataOut As New sSettings
            Dim XSS As New XmlSerializer(GetType(sSettings))
            Dim Reader As New FileStream(Application.StartupPath & "\data\settings.xml", FileMode.Open)

            DataOut = XSS.Deserialize(Reader)
            Reader.Close()

            Return DataOut

        Catch ex As Exception
            MsgBox("Failed to read settings file." & vbNewLine & ex.ToString())
            Return Nothing
        End Try

    End Function

    Public Sub defaultSettings()
        iSettings.txtShading = True
        iSettings.txtFocus = True
        iSettings.settSave = True
        iSettings.txtLoad = False
        iSettings.fileSave = False
    End Sub
End Module
