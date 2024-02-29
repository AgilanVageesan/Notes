Sub CopyModelAndSerial()
    Dim mainSheet As Worksheet
    Dim jamSheet As Worksheet
    Dim lastRowMain As Long
    Dim lastRowJam As Long
    Dim emailIndexMain As Long
    Dim emailIndexJam As Long
    Dim modelIndexJam As Long
    Dim serialIndexJam As Long
    
    ' Set the main sheet (first sheet of the workbook)
    Set mainSheet = ThisWorkbook.Sheets(1)
    
    ' Find the last row in the main sheet
    lastRowMain = mainSheet.Cells(mainSheet.Rows.Count, "A").End(xlUp).Row
    
    ' Find the column index of "Work Email" in the main sheet
    emailIndexMain = Application.Match("Work Email", mainSheet.Rows(1), 0)
    
    ' Loop through all sheets and copy data
    For Each jamSheet In ThisWorkbook.Sheets
        ' Check if the sheet name starts with "jam"
        If Left(jamSheet.Name, 3) = "jam" Then
            ' Find the last row in the jam sheet
            lastRowJam = jamSheet.Cells(jamSheet.Rows.Count, "A").End(xlUp).Row
            
            ' Find the column index of "Email Address" in the jam sheet
            emailIndexJam = Application.Match("Email Address", jamSheet.Rows(1), 0)
            
            ' Find the column index of "Model" in the jam sheet
            modelIndexJam = Application.Match("Model", jamSheet.Rows(1), 0)
            
            ' Find the column index of "Serial Number" in the jam sheet
            serialIndexJam = Application.Match("Serial Number", jamSheet.Rows(1), 0)
            
            ' Loop through each row in the jam sheet and copy data to the main sheet
            For i = 2 To lastRowJam ' Assuming headers are in the first row
                ' Find the corresponding email address in the main sheet
                emailJam = jamSheet.Cells(i, emailIndexJam).Value
                matchRowMain = Application.Match(emailJam, mainSheet.Columns(emailIndexMain), 0)
                
                ' If a match is found, copy the "Model" and "Serial Number" data to the main sheet
                If Not IsError(matchRowMain) Then
                    mainSheet.Cells(matchRowMain, emailIndexMain + 1).Value = jamSheet.Cells(i, modelIndexJam).Value
                    mainSheet.Cells(matchRowMain, emailIndexMain + 2).Value = jamSheet.Cells(i, serialIndexJam).Value
                End If
            Next i
        End If
    Next jamSheet
End Sub
