<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GettingStarted.aspx.cs" Inherits="ADOEF.Chapter_2.GettingStarted" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Getting Started with the ADO.NET Entity Framework</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns=
        "False" DataKeyNames="EmployeeID"
        DataSourceID="EntityDataSource1" BorderColor="Black" BorderStyle="Solid" Width="400px">
        <Columns>
        <asp:BoundField DataField="EmployeeID" HeaderText="Employee ID" ReadOnly="True" SortExpression="EmployeeID" />
        <asp:BoundField DataField="FirstName" HeaderText=
        "First Name" SortExpression="FirstName" />
        <asp:BoundField DataField="LastName" HeaderText=
        "Last Name" SortExpression="LastName" />
        <asp:BoundField DataField="Address" HeaderText="Address" SortExpression="Address" />
        </Columns>
        </asp:GridView>
        
        <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=PayrollEntities" 
           DefaultContainerName="PayrollEntities" 
           EntitySetName="Payroll">         
        </asp:EntityDataSource>

    </div>
    </form>
</body>
</html>
