<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM502000.aspx.cs" Inherits="Pages_LM502000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="ExternalLogisticsAPI.Graph.LUMDCLImportProc"
        PrimaryView="DocFilter">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="DocFilter" Width="100%" Height="100px" AllowAutoHide="false">
        <Template>
            <px:PXDateTimeEdit runat="server" ID="edRevFrom" DataField="Received_from" CommitChanges="True" />
            <px:PXDateTimeEdit runat="server" ID="edRevTo" DataField="Received_to" CommitChanges="True"/>
            <px:PXDropDown runat="server" ID="edCustomer_number" DataField="Customer_number" CommitChanges="True" Size="S"/>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ImportOrderList">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Width="40" Type="CheckBox" TextAlign="Center" CommitChanges="True" ></px:PXGridColumn>
                    <px:PXGridColumn DataField="LineNumber"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderDate"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderStatusID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderAmount"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SalesTaxAmt"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Processed"></px:PXGridColumn>
<%--                    <px:PXGridColumn DataField="LUMVendCntrlProcessLog__AcumaticaOrderID"/>
                    <px:PXGridColumn DataField="LUMVendCntrlProcessLog__ErrorDesc"/>--%>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
    </px:PXGrid>
</asp:Content>
