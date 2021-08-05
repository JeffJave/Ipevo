<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM202000.aspx.cs" Inherits="Page_LM202000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="IpevoCustomizations.Graph.LUMStateZipCodeMaint" PrimaryView="StateZipCode">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid AllowPaging="True" AdjustPageSize="Auto" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="StateZipCode">
			    <Columns>
				<px:PXGridColumn DataField="CountryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="State" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="State_State_name" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ZipCode" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CountyName" Width="140" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>