Sub CopyMobileNumbers()
    Dim apptioSheet As Worksheet
    Dim masterSheet As Worksheet
    Dim apptioLastRow As Long
    Dim masterLastRow As Long
    Dim apptioRow As Long
    Dim masterRow As Long
    Dim mobileNumbers As String
    Dim emailAddress As String
    Dim isFirstMatch As Boolean
    
    ' Set references to the relevant worksheets
    Set apptioSheet = Worksheets("SheetNameStartingWithApptio") ' Replace with the actual name of the sheet starting with "Apptio"
    Set masterSheet = Worksheets("Sheet1")
    
    ' Find the last row in each sheet
    apptioLastRow = apptioSheet.Cells(apptioSheet.Rows.Count, "A").End(xlUp).Row
    masterLastRow = masterSheet.Cells(masterSheet.Rows.Count, "A").End(xlUp).Row
    
    ' Loop through each row in the apptio sheet
    For apptioRow = 2 To apptioLastRow ' Assuming row 1 is header row
        ' Check if the service type is "MobilePlan"
        If apptioSheet.Cells(apptioRow, "Service Type").Value = "MobilePlan" Then
            ' Initialize mobileNumbers variable
            mobileNumbers = ""
            emailAddress = apptioSheet.Cells(apptioRow, "apptio.Email Address").Value
            isFirstMatch = True ' Flag to check if it's the first match for an email address
            
            ' Loop through each row in the master sheet to find matching email address
            For masterRow = 2 To masterLastRow ' Assuming row 1 is header row
                If masterSheet.Cells(masterRow, "master.Email-Primary work").Value = emailAddress Then
                    ' Check if mobile number is not empty
                    If Not IsEmpty(masterSheet.Cells(masterRow, "Sheet1.Mobile number").Value) Then
                        ' Add comma if mobileNumbers is not empty and it's not the first match
                        If mobileNumbers <> "" And Not isFirstMatch Then
                            mobileNumbers = mobileNumbers & ", "
                        End If
                        ' Add mobile number to mobileNumbers
                        mobileNumbers = mobileNumbers & masterSheet.Cells(masterRow, "Sheet1.Mobile number").Value
                        isFirstMatch = False ' Set flag to False after first match
                    End If
                End If
            Next masterRow
            
            ' Update the master sheet with concatenated mobileNumbers
            masterSheet.Cells(masterRow, "Sheet1.Mobile number").Value = mobileNumbers
        End If
    Next apptioRow
End Sub
