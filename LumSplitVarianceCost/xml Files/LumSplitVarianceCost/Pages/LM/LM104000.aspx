<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM104000.aspx.cs" Inherits="Page_LM104000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumSplitVarianceCost.Graph.STDCostVarPreferenceMaint"
        PrimaryView="lumSTDCostVarSetupView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXTab DataMember="lumSTDCostVarSetupView" ID="tab" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="General Setting">
                              <Template>
                                        <px:PXLayoutRule GroupCaption="ACCOUNT SETTING" runat="server" ID="CstPXLayoutRule1" StartGroup="True" LabelsWidth="L" ControlSize="" ></px:PXLayoutRule>
                                          <px:PXSelector runat="server" ID="CstPXSelector1" DataField="VarAcctID" AllowEdit="True" ></px:PXSelector>
                                          <px:PXSelector runat="server" ID="CstPXSelector2" DataField="InvtAcctID" AllowEdit="True" ></px:PXSelector>
                                          <px:PXSelector runat="server" ID="CstPXSelector3" DataField="InvtSubID" AllowEdit="True" ></px:PXSelector>
                                          <px:PXSelector runat="server" ID="CstPXSelector4" DataField="InvtSplitSubID" AllowEdit="True" ></px:PXSelector>
                              </Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXTab>
</asp:Content>