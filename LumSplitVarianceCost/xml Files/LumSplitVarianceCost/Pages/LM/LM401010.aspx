<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM401010.aspx.cs" Inherits="Page_LM401010" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumSplitVarianceCost.Graph.CostSplitMaint"
        PrimaryView="DetailsView">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterFilter" Width="100%" Height="50px" AllowAutoHide="false">
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="FinPeriodID"></px:PXSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="DetailsView">
                <RowTemplate></RowTemplate> 
                <Columns>
                    <px:PXGridColumn TextAlign="Center" Type="CheckBox" AllowCheckAll="True" DataField="UsrSplited" Width="30"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPeriodID" Width="72"></px:PXGridColumn>
                    <px:PXGridColumn DataField="BatchNbr" Width="140" LinkCommand="viewBatch"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Module" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Status" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CuryID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="BatchDescription" Width="280"></px:PXGridColumn>
                    <px:PXGridColumn DataField="GLTranID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="GLTranType" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="GLReleased" Width="60"></px:PXGridColumn>
                    <px:PXGridColumn DataField="AccountCD" Width="120" LinkCommand="viewDetails"></px:PXGridColumn>
                    <px:PXGridColumn DataField="AccountDescription" Width="220"></px:PXGridColumn>
                    <px:PXGridColumn DataField="InventoryCD" Width="140"></px:PXGridColumn>
                    <px:PXGridColumn DataField="INDescr" Width="280"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Qty" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CreditAmt" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="DebitAmt" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="StdCost" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="UnitCost" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="StdCostDate" Width="90"></px:PXGridColumn>
                    <px:PXGridColumn DataField="STDCostVariance" Width="100"></px:PXGridColumn>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
    </px:PXGrid>
</asp:Content>