<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM506000.aspx.cs" Inherits="Pages_LM506000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="IpevoCustomizations.Graph.LumTaxExemptionMaint"
        PrimaryView="UploadFileList">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="UploadFileList">
                <Columns>
                    <px:PXGridColumn DataField="FilePath"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="LumTaxExemptionCcertificateLog__IsProcess" />
                    <px:PXGridColumn DataField="LumTaxExemptionCcertificateLog__ErrorMsg" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
        <Mode AllowUpdate="false"/>
    </px:PXGrid>

    <script type="text/javascript">

</script>

</asp:Content>
