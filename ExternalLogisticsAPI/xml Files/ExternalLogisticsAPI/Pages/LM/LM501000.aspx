<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM501000.aspx.cs" Inherits="Page_LM501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="ExternalLogisticsAPI.LUM3DCartImportProc" PrimaryView="Filter">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="50px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit1" DataField="StartDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" />
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit2" DataField="EndDate" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<script type="text/javascript">
        window.onload = function () {
            window.setTimeout(function () {
                var Prepare = document.querySelectorAll('div[data-cmd="prepareRecords"]')[0].parentNode;
                var Import = document.querySelectorAll('div[data-cmd="Process"]')[0].parentNode;
                Import.parentNode.insertBefore(Prepare, Import);
            }, 100);
        };
    </script>
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ImportOrderList">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" DataField="Selected" Width="40" Type="CheckBox" TextAlign="Center" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InvoiceNumber" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderStatusID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SalesTaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LastUpdated" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BillingEmailID" Width="220" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>