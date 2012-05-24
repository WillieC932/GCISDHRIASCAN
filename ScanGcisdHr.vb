Imports System
Imports System.Text
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports Emc.InputAccel.ScanPlus.Scripting
Imports Emc.InputAccel.QuickModule.ClientScriptingInterface
Imports System.Data.OleDb



Namespace ScanGcisdHr


    <Serializable()> Public Class MyCustomParams

        Private _cp1 As String

        Public Sub New()
            _cp1 = "No Value"
        End Sub

        <Category("Database Path"), Description("Enter the full path to the validation database."), _
        PasswordPropertyText(False)> _
        Public Property CustomParameterOne() As String
            Get
                Return _cp1
            End Get
            Set(ByVal value As String)
                _cp1 = value
            End Set
        End Property

    End Class



    Public Class ScanPlusModuleEvents

        Implements IScanPlusModuleEvents

        Public Sub BeforeNewBatch(ByVal settings As INewBatchParameters, ByVal autoBatchCreation As Boolean) Implements IScanPlusModuleEvents.BeforeNewBatch

        End Sub

        Public Function BatchCreationError(ByVal settings As INewBatchParameters, ByVal autoBatchCreation As Boolean, ByVal errorInfo As IBatchCreationError) As Boolean Implements IScanPlusModuleEvents.BatchCreationError
            Return False
        End Function

        Public Function FilterProcessList(ByVal process As IBatchInformation) As Boolean Implements IScanPlusModuleEvents.FilterProcessList
            Return True
        End Function

        Public Function FilterBatchList(ByVal batch As IBatchInformation) As Boolean Implements IScanPlusModuleEvents.FilterBatchList
            Return True
        End Function

        Public Sub ModuleStart(ByVal arg As IApplication) Implements IDefaultModuleEvents.ModuleStart
        End Sub

        Public Sub ModuleFinish() Implements IDefaultModuleEvents.ModuleFinish
        End Sub

        Public Sub GotServerConnection(ByVal serverInformation As IServerInformation) Implements IDefaultModuleEvents.GotServerConnection
        End Sub

        Public Sub LostServerConnection(ByVal serverHostName As String) Implements IDefaultModuleEvents.LostServerConnection
        End Sub
    End Class


    Public Class ScanPlusTaskEvents

        Implements IScanPlusTaskEvents


        Public Function PrepareTask(ByVal taskInfo As ITaskEventArg, ByVal levelCreateInformation As ILevelCreationParameters) As ScriptResult Implements IScanPlusTaskEvents.PrepareTask
        End Function

        Public Function BeforeScan(ByVal taskInfo As ITaskEventArg, ByVal selectedDriver As IScanDriver) As ScriptResult Implements IScanPlusTaskEvents.BeforeScan
        End Function

        Public Function AfterScan(ByVal taskInfo As ITaskEventArg, ByVal selectedDriver As IScanDriver) As ScriptResult Implements IScanPlusTaskEvents.AfterScan
        End Function

        Public Sub ScanConfigurationSelected(ByVal taskInfo As ITaskEventArg, ByVal selectedDriver As IScanDriver) Implements IScanPlusTaskEvents.ScanConfigurationSelected
        End Sub

        Public Function BeforeNewLevel(ByVal taskInfo As ITaskEventArg, ByVal parentNode As IBatchNode, ByVal previousNode As IBatchNode, ByVal levelCreateInformation As ILevelCreationParameters, ByRef action As EventAction, ByRef discard As Boolean) As ScriptResult Implements IScanPlusTaskEvents.BeforeNewLevel
        End Function

        Public Function AfterNewLevel(ByVal taskInfo As ITaskEventArg, ByVal newNode As IBatchNode) As ScriptResult Implements IScanPlusTaskEvents.AfterNewLevel
        End Function

        Public Function LevelFinished(ByVal taskInfo As ITaskEventArg, ByVal finishedNode As IBatchNode) As ScriptResult Implements IScanPlusTaskEvents.LevelFinished
        End Function

        Public Function AfterPageAdded(ByVal taskInfo As ITaskEventArg, ByVal pageNode As IBatchNode) As ScriptResult Implements IScanPlusTaskEvents.AfterPageAdded
        End Function

        Public Function BeforeNodeDeleted(ByVal taskInfo As ITaskEventArg, ByVal node As IBatchNode) As ScriptResult Implements IScanPlusTaskEvents.BeforeNodeDeleted
        End Function

        <CustomParameterType(GetType(MyCustomParams))> _
        Public Function PrepopulateIndexes(ByVal taskInfo As ITaskEventArg, ByVal node As IBatchNode, ByVal fields() As IIndexField, ByVal initialIndexing As Boolean) As ScriptResult Implements IScanPlusTaskEvents.PrepopulateIndexes

            Dim cp As MyCustomParams = taskInfo.CustomParameter.Value
            Dim oForm As Enter_SSN
            Dim Dataresult As String = Nothing 'This will hold a pipe delimited data string returned from DataLookup Function

            Try

                If initialIndexing = True Then

                    oForm = New Enter_SSN()
                    oForm.ShowDialog()

                    Dataresult = Replace(oForm.SSN, "-", "")
                    Dataresult = DataLookup(Dataresult, cp.CustomParameterOne)

                    Dim arrDataresult() As String = Dataresult.Split("|")

                    fields(0).Value = arrDataresult(0)
                    fields(1).Value = arrDataresult(1)
                    fields(2).Value = arrDataresult(2)
                    fields(3).Value = arrDataresult(3)
                    fields(4).Value = arrDataresult(4)
                    fields(5).Value = arrDataresult(5)
                    fields(6).Value = arrDataresult(6)
                    fields(7).Value = arrDataresult(7)


                End If

                oForm = Nothing

                Return ScriptResult.OK

            Catch ex As Exception

                Return ScriptResult.Cancel

                'Show error message and exit
                MsgBox(ex.Message & vbCrLf & ex.StackTrace)

            End Try

        End Function


        <CustomParameterType(GetType(MyCustomParams))> _
        Public Function IndexChanged(ByVal taskInfo As ITaskEventArg, ByVal node As IBatchNode, ByVal fields() As IIndexField, ByVal valueName As String, ByVal initialIndexing As Boolean) As ScriptResult Implements IScanPlusTaskEvents.IndexChanged



            If valueName = "Level_2_KeyEntry_0" Then

                Dim cp As MyCustomParams = taskInfo.CustomParameter.Value
                Dim myStep As IWorkflowStep
                Dim myApp As IScanPlusApplication
                Dim myValues As IIAValueProvider
                Dim oForm As Enter_SSN

                oForm = New Enter_SSN()
                oForm.ShowDialog()

                myApp = taskInfo.Application
                myStep = myApp.CurrentStep
                myValues = node.Values(myStep)

                Dim Dataresult As String = Nothing 'This will hold a pipe delimited data string returned from DataLookup Function

                Dim valSSN As String = fields(0).Value
                myValues.SetString("Level_2_KeyEntry_0", valSSN)
                'MessageBox.Show("SSN is: " & valSSN)

                Dataresult = Replace(oForm.SSN, "-", "")

                'MessageBox.Show("before datalookup: " & Dataresult)
                Dataresult = DataLookup(Dataresult, cp.CustomParameterOne)

                'MessageBox.Show("after datalookup: " & Dataresult)

                'MessageBox.Show("The value of mystep is: " & node.Values(myStep).GetString("Level_2_KeyEntry_0", "not found"))
                'MessageBox.Show("The value of mystep is: " & node.Values(myStep).GetString("Level_2_KeyEntry_1", "not found"))
                'MessageBox.Show("The value of mystep is: " & node.Values(myStep).GetString("Level_2_KeyEntry_2", "not found"))



                Dim arrDataresult() As String = Dataresult.Split("|")

                Dim mytask As IWorkflowTask = Nothing

                mytask.Value.SetString("Level_2_KeyEntry_0", arrDataresult(0))
                mytask.Value.SetString("Level_2_KeyEntry_1", arrDataresult(1))
                mytask.Value.SetString("Level_2_KeyEntry_1", arrDataresult(1))

                'myValues.SetString("Level_2_KeyEntry_0", arrDataresult(0))
                node.Values(myStep).SetString("Level_2_KeyEntry_0", arrDataresult(0))
                'myValues.SetString("Level_2_KeyEntry_1", arrDataresult(1))
                node.Values(myStep).SetString("Level_2_KeyEntry_1", arrDataresult(1))
                'myValues.SetString("Level_2_KeyEntry_2", arrDataresult(2))
                node.Values(myStep).SetString("Level_2_KeyEntry_2", arrDataresult(2))

                fields(0).Value = arrDataresult(0)
                fields(1).Value = arrDataresult(1)
                fields(2).Value = arrDataresult(2)
                fields(3).Value = arrDataresult(3)
                fields(4).Value = arrDataresult(4)
                fields(5).Value = arrDataresult(5)
                fields(6).Value = arrDataresult(6)
                fields(7).Value = arrDataresult(7)

                oForm = Nothing

                Return ScriptResult.OK


            End If


        End Function




        Public Function DataLookup(ByVal SSN As String, ByVal path As String) As String

            'Define the connectors
            Dim oConn As OleDbConnection = Nothing 'holds instance of DB connection
            Dim oComm As OleDbCommand = Nothing 'holds instance of DB command
            Dim oData As OleDbDataAdapter = Nothing 'holds instance of Data Adapter
            Dim resultSet As New DataSet 'holds entire data set
            Dim oConnect As String = Nothing 'holds provider information
            Dim oQuery As String = Nothing 'holds SQL query
            Dim oRecordset As OleDbDataReader 'variable for data reader
            Dim dbRecordCount As Integer 'number of records in database
            Dim oTemp As String = Nothing 'the SSN of each record as it is being read; used to compare to SSN searched for
            Dim i As Integer 'Counter
            Dim a As String = Nothing 'variable for SSN
            Dim b As String = Nothing 'variable for Last Name
            Dim c As String = Nothing 'variable for First Name
            Dim d As Object = Nothing 'variable for MI
            Dim e As Object = Nothing 'variable for Former Name
            Dim f As String = Nothing 'variable for Employee Type
            Dim g As String = Nothing 'variable for Blg & Blg Description
            Dim h As String = Nothing 'variable for Status
            Dim strResult As String = Nothing ' holds return values out of function; fields are pipe delimited

            'Define connection string
            oConnect = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & path

            'Define the query Strings
            oQuery = "SELECT * FROM tblEmpData"

            Try

                'Instantiate the connection and open it
                oConn = New OleDbConnection(oConnect)

                oConn.Open()

                'Instantiate the command and adapter
                oComm = New OleDbCommand(oQuery, oConn)

                oData = New OleDbDataAdapter(oQuery, oConn)

                oRecordset = oComm.ExecuteReader

                'Fill dataset; resultSet contains entire table
                oData.Fill(resultSet)

                dbRecordCount = resultSet.Tables(0).Rows.Count 'get the number of records in the table; this code only works with one table in database

                For i = 1 To dbRecordCount

                    While oRecordset.Read() 'reads through the table one line at a time

                        oTemp = oRecordset.Item(0) 'oTemp holds that value of the SSN field for the currently read record

                        If oTemp.Contains(SSN) Then

                            a = oRecordset.Item(0)
                            b = oRecordset.Item(1)
                            c = oRecordset.Item(2)
                            d = oRecordset.Item(3) 'using object type because MI is optional and may return DBNull value
                            d = d.ToString()
                            e = oRecordset.Item(4) 'using object type because Former Name is optional and may return DBNull value
                            e = e.ToString()
                            f = oRecordset.Item(5)
                            g = oRecordset.Item(6)
                            h = oRecordset.Item(7)

                        End If

                    End While

                Next

                strResult = a & "|" & b & "|" & c & "|" & d & "|" & e & "|" & f & "|" & g & "|" & h

                'Close connection
                oConn.Close()

            Catch ex As OleDb.OleDbException
            Catch ex As Exception

                'Show error message and exit
                MsgBox(ex.Message & vbCrLf & ex.StackTrace)

            Finally

                'Dispose the connector objects
                If Not (oConn Is Nothing) Then oConn.Dispose()
                oConn = Nothing
                If Not (oComm Is Nothing) Then oComm.Dispose()
                oComm = Nothing
                If Not (oData Is Nothing) Then oData.Dispose()
                oData = Nothing

            End Try

            Return strResult

        End Function

        Public Function ValidateIndexes(ByVal taskInfo As ITaskEventArg, ByVal node As IBatchNode, ByVal fields() As IIndexField, ByVal initialIndexing As Boolean) As ScriptResult Implements IScanPlusTaskEvents.ValidateIndexes
        End Function

        Public Function BeforeBatchClose(ByVal taskInfo As ITaskEventArg, ByVal autoBatchCreation As Boolean, ByRef closeMode As BatchCloseMode) As ScriptResult Implements IScanPlusTaskEvents.BeforeBatchClose
        End Function

        Public Function AutoBatchCreation(ByVal taskInfo As ITaskEventArg, ByVal batchParams As INewBatchParameters) As ScriptResult Implements IScanPlusTaskEvents.AutoBatchCreation
        End Function

        Public Function FilterProcessList(ByVal taskInfo As ITaskEventArg, ByVal process As IBatchInformation) As Boolean Implements IScanPlusTaskEvents.FilterProcessList
            Return True
        End Function

    End Class

End Namespace
