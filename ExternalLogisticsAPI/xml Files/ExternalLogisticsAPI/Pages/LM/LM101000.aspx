<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM101000.aspx.cs" Inherits="Page_LM101000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="ExternalLogisticsAPI.LUMSalesChanelSetup"
        PrimaryView="ThreeDCart">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXTab DataMember="ThreeDCart" ID="tab" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" AllowAutoHide="false">
        <Items>
            <px:PXTabItem Text="3D Cart">
                <Template>
                    <px:PXLayoutRule ControlSize="" runat="server" ID="CstPXLayoutRule4" StartGroup="True" GroupCaption="API Settings"></px:PXLayoutRule>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit1" DataField="SecureURL"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit2" DataField="ClientID"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit3" DataField="ClientSecret"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="AuthToken"></px:PXTextEdit>
                    <px:PXLayoutRule ControlSize="" GroupCaption="Integration Settings" runat="server" ID="CstPXLayoutRule6" StartGroup="True"></px:PXLayoutRule>
                    <px:PXSelector runat="server" ID="CstPXSelector8" DataField="OrderType" />
                    <px:PXSegmentMask runat="server" ID="CstPXSegmentMask7" DataField="CustomerID" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Vendor Central">
                <Template>
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRuleG1" StartColumn="True" />
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
                    <px:PXFormView ID="CstformSetup" runat="server" DataSourceID="ds" Width="100%" DataMember="VendorCentral" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="API Settings"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="SecureURL"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="SecureURLbatches" />
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit11" DataField="ClientID"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit12" DataField="ClientSecret"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="edAuthType" DataField="AuthType"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit13" DataField="AuthToken"></px:PXTextEdit>
                            <px:PXLayoutRule StartColumn="False" ColumnSpan="1" ControlSize="S" GroupCaption="Integration Settings" runat="server" ID="CstPXLayoutRule14" StartGroup="True"></px:PXLayoutRule>
                            <px:PXSelector runat="server" ID="CstPXSelector15" DataField="OrderType"></px:PXSelector>
                            <px:PXSegmentMask runat="server" ID="CstPXSegmentMask16" DataField="CustomerID"></px:PXSegmentMask>
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="YUSEN CA">
                <Template>
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRuleG1" StartColumn="True" />
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
                    <px:PXFormView ID="CstformSetup" runat="server" DataSourceID="ds" Width="100%" DataMember="CAYusen" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="FTP Settings"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edFtpHost" DataField="FtpHost" />
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="FtpUser" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit1" DataField="FtpPass" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit2" DataField="FtpPath" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit3" DataField="FtpPort" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="3PL UK">
                <Template>
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRuleG1" StartColumn="True" />
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
                    <px:PXFormView ID="CstformSetup" runat="server" DataSourceID="ds" Width="100%" DataMember="UKP3PL" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="FTP Settings"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="edFtpHost" DataField="FtpHost" />
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="FtpUser" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit1" DataField="FtpPass" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit99" DataField="FtpPath" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit4" DataField="FtpOutPath" />
                            <px:PXTextEdit runat="server" ID="PXTextEdit3" DataField="FtpPort" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="MIDDLE WARE">
                <Template>
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRuleG1" StartColumn="True" />
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
                    <px:PXFormView ID="CstformSetup" runat="server" DataSourceID="ds" Width="100%" DataMember="Middleware" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="API Settings"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="SecureURL_login"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="SecureURL_fbm" />
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit11" DataField="ClientID"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit12" DataField="ClientSecret"></px:PXTextEdit>
                            <px:PXLayoutRule StartColumn="False" ColumnSpan="1" ControlSize="S" GroupCaption="Integration Settings" runat="server" ID="CstPXLayoutRule14" StartGroup="True"></px:PXLayoutRule>
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="LOB">
                <Template>
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRuleG1" StartColumn="True" />
                    <px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
                    <px:PXFormView ID="CstformSetup" runat="server" DataSourceID="ds" Width="100%" DataMember="LobAPI" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule ControlSize="" runat="server" ID="CstPXLayoutRule4" StartGroup="True" GroupCaption="API Settings"></px:PXLayoutRule>
	<px:PXDropDown runat="server" ID="CstPXDropDownLOBisProd" DataField="IsProd" ></px:PXDropDown>
                            <px:PXTextEdit runat="server" ID="CstPXTextEditUrl" DataField="Lobapiurl"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEditAuthT" DataField="AuthCode_Test"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEditAuthP" DataField="AuthCode_Prod"></px:PXTextEdit>
                            <px:PXLayoutRule ControlSize="" GroupCaption="Mail Content Settings" runat="server" ID="CstPXLayoutRule6" StartGroup="True"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="CstPXSelectorShipName" DataField="ShipFrom_Name" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEditShipAddr" DataField="ShipFrom_Address" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEditShipCity" DataField="ShipFrom_City" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEditShipState" DataField="ShipFrom_State" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEditShipCountry" DataField="ShipFrom_Country" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEditShipZip" DataField="ShipFrom_Zip" ></px:PXTextEdit></Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="200"></AutoSize>
    </px:PXTab>
</asp:Content>