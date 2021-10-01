<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM501010.aspx.cs" Inherits="Page_LM501010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="ExternalLogisticsAPI.Graph.LUMAmzInterfaceAPIMaint" PrimaryView="AMZInterfaceAPI">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<%--<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="50px" AllowAutoHide="false">
		<Template>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit2" DataField="StartDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit1" DataField="EndDate" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>--%>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid AllowPaging="True" AdjustPageSize="Auto" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="AMZInterfaceAPI">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" DataField="Selected" Width="40" Type="CheckBox" TextAlign="Center" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderType" Width="250" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderNbr" Width="160" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SequenceNo" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Marketplace" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Data1" Width="400" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Data2" Width="250" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Write2Acumatica1" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Write2Acumatica2" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Remark" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedDateTime" Width="130" DisplayFormat="g" ></px:PXGridColumn>
			    </Columns>
				<RowTemplate>
					<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask4" DataField="BranchID" ></px:PXSegmentMask>
                </RowTemplate>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>