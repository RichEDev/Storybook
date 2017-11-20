Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Web

Public Class Crypto

    Private bytIV() As Byte = _
    {12, 241, 10, 21, 90, 74, 11, 39, 9, 91, 45, 78, 189, 211, 133, 62, 121, 22, 101, 34, 90, 74, 121, 39, 93, 9, 45, 78, 1, 211, 33, 162}

    Public Enum Providers
        DES
        RC2
        Rijndael
    End Enum

    Private _CryptoService As SymmetricAlgorithm

    Public Sub New(ByVal NetSelected As Providers)
        Select Case NetSelected
            Case Providers.DES
                _CryptoService = New DESCryptoServiceProvider
            Case Providers.RC2
                _CryptoService = New RC2CryptoServiceProvider
            Case Providers.Rijndael
                _CryptoService = New RijndaelManaged
        End Select
    End Sub

    Public Sub New(ByVal ServiceProvider As SymmetricAlgorithm)
        _CryptoService = ServiceProvider
    End Sub

    Private Function GetLegalKey(ByVal Key As String) As Byte()

        Dim sTemp As String
        If (_CryptoService.LegalKeySizes.Length > 0) Then
            Dim maxSize As Integer = _CryptoService.LegalKeySizes(0).MaxSize
            If Key.Length * 8 > maxSize Then
                sTemp = Key.Substring(0, (maxSize / 8))
            Else
                Dim moreSize As Integer = _CryptoService.LegalKeySizes(0).MinSize
                Do While (Key.Length * 8 > moreSize)
                    moreSize += _CryptoService.LegalKeySizes(0).SkipSize
                Loop
                sTemp = Key.PadRight(moreSize / 8, "X")
            End If
        Else
            sTemp = Key
        End If

        If (_CryptoService.LegalBlockSizes.Length > 0) Then
            Dim maxSize As Integer = _CryptoService.LegalBlockSizes(0).MaxSize
            ReDim Preserve bytIV(sTemp.Length - 1)
            If sTemp.Length * 8 > maxSize Then
                ReDim Preserve bytIV(maxSize / 8 - 1)
            End If
        End If

        Return ASCIIEncoding.ASCII.GetBytes(sTemp)
    End Function

    Public Function Encrypt(ByVal Source As String, ByVal Key As String) As String
        Dim bytIn As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(System.Web.HttpUtility.UrlEncode(Source))
        Dim ms As MemoryStream = New MemoryStream


        _CryptoService.Key = GetLegalKey(Key)
        _CryptoService.IV = bytIV

        Dim encrypto As ICryptoTransform = _CryptoService.CreateEncryptor()

        Dim cs As CryptoStream = New CryptoStream(ms, encrypto, CryptoStreamMode.Write)

        cs.Write(bytIn, 0, bytIn.Length)
        cs.FlushFinalBlock()
        cs.Close()
        Dim bytOut() As Byte = ms.ToArray()
        ms.Close()

        Return Convert.ToBase64String(bytOut)
    End Function

    Public Function Decrypt(ByVal Source As String, ByVal Key As String) As String
        Dim bytIn As Byte() = System.Convert.FromBase64String(Source)
        Dim ms As MemoryStream = New MemoryStream(bytIn)

        Dim bytKey() As Byte = GetLegalKey(Key)
        Dim bytTemp(bytIn.Length) As Byte

        _CryptoService.Key = bytKey
        _CryptoService.IV = bytIV

        Dim encrypto As ICryptoTransform = _CryptoService.CreateDecryptor()


        Dim cs As CryptoStream = New CryptoStream(ms, encrypto, CryptoStreamMode.Read)
        Dim output As String
        Try

            Dim sr As New StreamReader(cs)
            output = sr.ReadToEnd
            sr.Close()
            ms.Close()
            cs.Close()
        Catch ex As Exception
        End Try
        Return System.Web.HttpUtility.UrlDecode(output)
    End Function

End Class