<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM401020.aspx.cs" Inherits="Page_LM401020" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumSplitVarianceCost.Graph.STDCostVarianceSplitKitMaint"
        PrimaryView="MasterFilter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterFilter" Width="100%" Height="50px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="FromFinPeriodID" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector3" DataField="ToFinPeriodID" ></px:PXSelector></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn DataField="FinPeriodID" Width="72" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AccountCD" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AccountDescription" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SubID" Width="72" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GLTranType" Width="70" />
				<px:PXGridColumn DataField="InventoryCD" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryDescr" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KitInventoryCD" Width="140" />
				<px:PXGridColumn DataField="KitInventoryDescr" Width="280" />
				<px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SplitQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Split" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VarianceCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SplitCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="STDCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="INCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NewSTDCost" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>