<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM503000.aspx.cs" Inherits="Pages_LM503000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="ExternalLogisticsAPI.Graph.LUMWarehouseImportProc"
        PrimaryView="ImportShipmentList">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<%--<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="DocFilter" Width="100%" Height="100px" AllowAutoHide="false">
        <Template>
            <px:PXDateTimeEdit runat="server" ID="edRevFrom" DataField="Received_from" CommitChanges="True" />
            <px:PXDateTimeEdit runat="server" ID="edRevTo" DataField="Received_to" CommitChanges="True" />
            <px:PXDropDown runat="server" ID="edCustomer_number" DataField="Customer_number" CommitChanges="True" Size="S" />
        </Template>
    </px:PXFormView>
</asp:Content>--%>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ImportShipmentList">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Width="40" Type="CheckBox" TextAlign="Center" CommitChanges="True" AllowCheckAll="True"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Erporder"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="TrackingNbr"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Carrier"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentDate"></px:PXGridColumn>
                    <px:PXGridColumn DataField="LUMWarehouseImportProcessLog__IsProcess" />
                    <px:PXGridColumn DataField="LUMWarehouseImportProcessLog__ProcessMessage" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
        <Mode AllowUpdate="false" AllowUpload="True" />
    </px:PXGrid>

    <script type="text/javascript">

</script>

</asp:Content>
