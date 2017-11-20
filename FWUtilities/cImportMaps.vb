Public Class cImportMaps
    Public Sub SaveImport(ByVal fileName As String, ByVal importMap As Object)
        'Open file stream - make sure it's a file stream, not a text formatted one, otherwise you can't use binary!
        Dim fStream As System.IO.Stream = New System.IO.FileStream(fileName, IO.FileMode.Create)

        'Create a binary serializer object
        Dim formatter As System.Runtime.Serialization.IFormatter = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

        'Format the import map into binary using the Binary Formatter, point it at the file stream to dump to.
        formatter.Serialize(fStream, importMap)

        'Free up the file handle
        fStream.Close()

        'Free up memory pointers now, Don't wait for Garbage Collector or the FWUtilities GUI will hang
        formatter = Nothing
        fStream = Nothing
    End Sub

    Public Function GetImport(ByVal fileName As String) As Object
        'Open file stream - make sure it's a file stream, not a text formatted one, otherwise you can't use binary!
        Dim fStream As System.IO.Stream = New System.IO.FileStream(fileName, IO.FileMode.Open)

        'Create a binary serializer object
        Dim formatter As System.Runtime.Serialization.IFormatter = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

        'Read the stream into the binary serializer
        Dim dataIn As Object = formatter.Deserialize(fStream)

        'Free up the file handle
        fStream.Close()

        'Free up memory pointers now, Don't wait for Garbage Collector or the FWUtilities GUI will hang
        formatter = Nothing
        fStream = Nothing

        Return dataIn
    End Function

    <Serializable()> _
    Public Structure ImportMapping
        Public IgnoreHeader As Boolean
        Public FileName As String
        Public DelimiterChar As String
        'Public ImpMap() As cUtility.ImportMap
        Public uiMapping() As cUtility.UserMappings
        Public uiMappingCount As Integer
        Public FixedMappings As System.Collections.SortedList
    End Structure
End Class
