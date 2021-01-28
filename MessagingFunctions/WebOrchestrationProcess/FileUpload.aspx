<%@ Page Title="FileUpload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FileUpload.aspx.cs" Inherits="WebOrchestrationProcess.FileUpload" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Press the button to start upload of your files of your directory</h3>
    <p>
        <a class="btn btn-default" runat="server" onserverclick="UploadFilesOfDirectory">Start &raquo;</a>
        <asp:Label id="labelReady" runat="server" Text="File Uploaded" Visible="false"/>
    </p>
</asp:Content>
