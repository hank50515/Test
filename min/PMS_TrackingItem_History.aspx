<%@ Register TagPrefix="uc" TagName="PMSSetView" Src="~/UserControl/PMS/UCPMS_TrackingItem_Set_View.ascx" %>
<%@ Register TagPrefix="uc" TagName="PMSWriteView" Src="~/UserControl/PMS/UCPMS_TrackingItem_Write_View.ascx" %>
<%@ Register TagPrefix="uc" TagName="PMSRdecView" Src="~/UserControl/PMS/UCPMS_TrackingItem_Rdec_View.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="panelHead" runat="Server">
    <div style="display: none">
        &nbsp;</div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="panelContentPlaceHolder" runat="Server">
    <script type="text/javascript" language="javascript">
        function DownLoadWord() {
            var PlanName = document.getElementById("PlanName").value;
            window.open("../UserControl/SubPages/preview_plan_export_word.aspx?PlanName=" + PlanName);
            //            window.document.location.reload(true);
            document.getElementById("DownLoadWord").value = "Y";
            __doPostBack("", "");
        }
        function PrintScreen(ClientID) {
　
            var divContent = document.getElementById(ClientID);
            var printPage = window.open('', 'printPage', '');
            printPage.document.open();
            printPage.document.write('<html><head>');
            printPage.document.write('<link rel="stylesheet" type="text/css" href="../css/sameStyle.css" /><link rel="StyleSheet" type="text/css" href="../css/table.css" /><link rel="StyleSheet" type="text/css" href="../css/extCustom.css" />');
            printPage.document.write('<style type="text/css">');
            printPage.document.write($("#LocalStyle").html());
            printPage.document.write('</style>');
            printPage.document.write('</head>');
            printPage.document.write('<body onload="window.print();window.close();">');
            //printPage.document.write('<body>');
            printPage.document.write('<PRE>');
            printPage.document.write('<div class="container"><div class="mainLayoutCol3"><div class="mainContent"><div class="mainbox">');
            printPage.document.write(divContent.innerHTML);
            printPage.document.write('</div></div></div></div>');
            printPage.document.write('</PRE>');
            printPage.document.close('</body></html>');
　
        }
        function DownloadLinkClick(a) {
            var allParameters = window.location.search.substring(1);
            var parameters = allParameters.split('&');
            var projectNo, workNo;
            for (i = 0; i < parameters.length; i++) {
                var parameter = parameters[i].split('=');
                if (parameter[0] == 'ProjectNo') projectNo = parameter[1];
                else if (parameter[0] == 'WorkNo') workNo = parameter[1];
            }
            var iframe = document.createElement('iframe');
            iframe.src = 'PMS_Download_File.aspx?functionName=DownloadLink&fileId=' + a.id + '&ProjectNo=' + projectNo + '&WorkNo=' + workNo;
            iframe.style.display = 'none';
            document.body.appendChild(iframe);
        }
    </script>
    <ext:Viewport ID="tabContentViewport" runat="server" Layout="FitLayout"  AutoDoLayout="true"
        AutoRender="true">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <West Collapsible="true" Split="true">
                    <ext:Panel ID="LeftPanel" Hidden="true" runat="server" Width="217" AutoScroll="true"
                        Layout="Accordion" Border="false" Collapsed="true">
                        <Content>
                        </Content>
                    </ext:Panel>
                </West>
                <Center>
                    <ext:Container ID="tabContainer2" runat="server" Region="None" AutoScroll="true">
                        <Items>
                            <ext:BorderLayout ID="BorderLayout2" runat="server">
                                <North>
                                    <ext:Panel ID="Panel2" Region="North" AutoHeight="true" runat="server" AutoScroll="false"
                                        Border="false" HideBorders="true" Collapsible="false">
                                        <Content>
                                            <div class="mainbox">
                                                <div class="function_bar" id="function_bar">
                                                    <ul>
                                                        <li><a href="#">
                                                            <img id="ImgToolSrc1" class="ImgToolSrc1" src="../images/function_bar_03.jpg" width="11"
                                                                height="33" alt="" /></a></li>
                                                        <li><a href="#">
                                                            <img id="ImgToolSrc2" class="ImgToolSrc2" src="../images/function_bar_02.jpg" width="35"
                                                                height="33" alt="" /></a></li>
                                                        <li>
                                                            <ext:Button ID="BtntSearch" runat="server" Cls="function_barV2_button">
                                                                <DirectEvents>
                                                                    <Click OnEvent="BtntSearch_OnClick">
                                                                        <EventMask ShowMask="true" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </li>
                                                        <li>
                                                            <ext:Button ID="ButtonPrint" runat="server" Cls="function_barV2_button" Text="直接列印"
                                                                OnClientClick="javascript:PrintScreen('divContent')">
                                                            </ext:Button>
                                                        </li>
                                                        <li>
                                                            <ext:Button ID="ButtonPrintWord" runat="server" Cls="function_barV2_button" Text="轉WORD"
                                                                OnDirectClick="BtnPrintWord_OnClick">
                                                            </ext:Button>
                                                        </li>
                                                        <li style="padding-top: 10px;">【如果Word無法開啟，請至首頁下載執行IE設定】</li>
                                                    </ul>
                                                </div>
                                                <div class="table_titleV2">
                                                    <ext:Label runat="server" ID="LblTitle" Text="">
                                                    </ext:Label>
                                                </div>
                                                <div>
                                                    <table width="100%" class="tableV2">
                                                        <tr>
                                                            <th width="120px" class="left">
                                                                <ext:Label ID="LblYear" Text="填報日期" runat="server">
                                                                </ext:Label>
                                                            </th>
                                                            <td width="120px">
                                                                <ext:ComboBox ID="CbbCycle" runat="server" SelectedIndex="0" Editable="false" Width="100">
                                                                    <Items>
                                                                        <ext:ListItem Text="當期" Value="0" />
                                                                        <ext:ListItem Text="日期區間" Value="1" />
                                                                    </Items>
                                                                    <DirectEvents>
                                                                        <Select OnEvent="CbbCycle_OnChange">
                                                                            <EventMask ShowMask="true" MinDelay="150" Msg="讀取中" />
                                                                        </Select>
                                                                    </DirectEvents>
                                                                </ext:ComboBox>
                                                            </td>
                                                            <td width="240px" nowrap="nowrap">
                                                                <ext:Panel ID="Panel1" runat="server" BaseCls="x-plain">
                                                                    <Items>
                                                                        <ext:TableLayout ID="TableLayout2" runat="server" Columns="3">
                                                                            <Cells>
                                                                                <ext:Cell>
                                                                                    <gpmnet:TaiwanDateField ID="SDate" runat="server" Disabled="true" Width="100" AllowBlank="false"
                                                                                        Vtype="daterange">
                                                                                        <CustomConfig>
                                                                                            <ext:ConfigItem Name="endDateField" Value="#{EDate}" Mode="Value" />
                                                                                        </CustomConfig>
                                                                                    </gpmnet:TaiwanDateField>
                                                                                </ext:Cell>
                                                                                <ext:Cell>
                                                                                    <ext:Label runat="server" ID="LblDt" Text="~" />
                                                                                </ext:Cell>
                                                                                <ext:Cell>
                                                                                    <gpmnet:TaiwanDateField ID="EDate" runat="server" Disabled="true" Width="100" Vtype="daterange">
                                                                                        <CustomConfig>
                                                                                            <ext:ConfigItem Name="startDateField" Value="#{SDate}" Mode="Value" />
                                                                                        </CustomConfig>
                                                                                    </gpmnet:TaiwanDateField>
                                                                                </ext:Cell>
                                                                            </Cells>
                                                                        </ext:TableLayout>
                                                                    </Items>
                                                                </ext:Panel>
                                                            </td>
                                                            <td>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </Content>
                                    </ext:Panel>
                                </North>
                                <Center>
                                    <ext:Panel ID="SouthPanel" runat="server" AutoScroll="true" Border="false" HideBorders="true"
                                        Collapsible="false">
                                        <Content>
                                            <ext:Panel runat="server" ID="PalContent" Border="false" HideBorders="true">
                                                <Content>
                                                </Content>
                                            </ext:Panel>
                                        </Content>
                                    </ext:Panel>
                                </Center>
                            </ext:BorderLayout>
                        </Items>
                    </ext:Container>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>