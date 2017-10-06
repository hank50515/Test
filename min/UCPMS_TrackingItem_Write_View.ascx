<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UCPMS_TrackingItem_Write_View.ascx.cs" Inherits="UserControl_UCPMS_TrackingItem_Write_View" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<ext:Panel ID="PanContent" runat="server" AutoHeight="true" Title="" Border="false" HideBorders="true" >    
    <Content>
<style type="text/css">
.link{ cursor:hand; text-decoration:underline; color:Blue;}
　
</style>
<div class="mainbox">
 <div class="table_titleV2"><ext:Label runat="server" ID="LblTitle" Text="主辦填報"></ext:Label></div>
<ext:Panel ID="TrackingItem_Write_View" runat="server" Title="" Border="false" HideBorders="true" ><Content></Content></ext:Panel>
<ext:Hidden runat="server" ID="HidDownLoadFileId"></ext:Hidden>
    <ext:Button runat="server" ID="BtnDownLoadFile" Hidden="true">
        <DirectEvents><Click OnEvent="BtnDownLoadFile_OnClick"></Click></DirectEvents>
    </ext:Button>
</div>
　
</Content>
</ext:Panel>