<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" CodeBehind="DownloadData.aspx.cs" Inherits="Unix_Web.Source.DownloadData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Download Data System
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .download-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 25px;
            padding: 20px;
        }

        .download-card {
            background: rgba(30, 41, 59, 0.6);
            backdrop-filter: blur(20px);
            border: 1px solid rgba(0, 212, 255, 0.2);
            border-radius: 20px;
            padding: 30px;
            text-align: center;
            transition: all 0.3s ease;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            min-height: 250px;
        }

        .download-card:hover {
            transform: translateY(-10px);
            border-color: var(--primary);
            box-shadow: 0 10px 30px rgba(0, 212, 255, 0.2);
        }

        .card-icon {
            font-size: 50px;
            margin-bottom: 15px;
        }

        .card-title {
            font-family: 'Orbitron', sans-serif;
            font-size: 20px;
            color: var(--primary);
            margin-bottom: 10px;
            font-weight: 700;
        }

        .card-desc {
            font-size: 13px;
            color: var(--text-dim);
            margin-bottom: 25px;
            line-height: 1.6;
        }

        .btn-download {
            background: linear-gradient(135deg, var(--primary) 0%, var(--secondary) 100%);
            color: var(--bg-darker);
            border: none;
            padding: 12px 30px;
            border-radius: 12px;
            font-weight: 700;
            cursor: pointer;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 10px;
            transition: all 0.3s;
            width: 100%;
            justify-content: center;
        }

        .btn-download:hover {
            box-shadow: 0 5px 20px rgba(0, 212, 255, 0.4);
            filter: brightness(1.1);
        }

        .checkbox-group {
            display: flex;
            gap: 15px;
            margin-bottom: 15px;
            background: rgba(0, 0, 0, 0.2);
            padding: 10px;
            border-radius: 8px;
            border: 1px solid rgba(0, 212, 255, 0.2);
            justify-content: center;
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            gap: 5px;
            color: var(--text-light);
            font-size: 13px;
            cursor: pointer;
        }

        .checkbox-item input[type="checkbox"] {
            accent-color: var(--primary);
            width: 16px;
            height: 16px;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="updMain" runat="server">
        <ContentTemplate>
            <div class="page-header" style="margin-bottom: 30px; padding-left: 20px;">
                <h1 style="font-family: 'Orbitron', sans-serif; color: white;">DATA EXPORT CENTER</h1>
                <p style="color: var(--text-dim);">Chọn biểu mẫu dữ liệu bạn cần xuất ra Excel</p>
            </div>

            <div class="download-container">
                <div class="download-card">
                    <div class="card-icon">📦</div>
                    <div>
                        <div class="card-title">INV035</div>
                        <div class="card-desc">成品 DOT CODE 維護作業<br />Factory: V | Subno: 4</div>
                    </div>
                    <asp:LinkButton ID="btnDownloadINV035" runat="server" CssClass="btn-download btn-action btn-excel" OnClick="btnDownloadINV035_Click">
                        <span>📥</span> DOWNLOAD EXCEL
                    </asp:LinkButton>
                </div>

                <div class="download-card">
                    <div class="card-icon">🏭</div>
                    <div>
                        <div class="card-title">CRDSI801</div>
                        <div class="card-desc">動靜平衡均一性測試標準</div>
                    </div>
                    <asp:LinkButton ID="btnDownloadCRDSI801" runat="server" CssClass="btn-download btn-action btn-excel" OnClick="btnDownloadCRDSI801_Click">
                        <span>📥</span> DOWNLOAD EXCEL
                    </asp:LinkButton>
                </div>

                <div class="download-card">
                    <div class="card-icon">🎛️</div>
                    <div>
                        <div class="card-title">RDSMEST</div>
                        <div class="card-desc">  <br />Chọn mã máy để xuất Excel:</div>
                        
                        <div class="checkbox-group">
                            <label class="checkbox-item">
                                <asp:CheckBox ID="chkTR0001" runat="server" /> TR0001
                            </label>
                            <label class="checkbox-item">
                                <asp:CheckBox ID="chkTR0002" runat="server" /> TR0002
                            </label>
                            <label class="checkbox-item">
                                <asp:CheckBox ID="chkSW0001" runat="server" /> SW0001
                            </label>
                        </div>
                    </div>
                    <asp:LinkButton ID="btnDownloadRDSMEST" runat="server" CssClass="btn-download btn-action btn-excel" OnClick="btnDownloadRDSMEST_Click">
                        <span>📥</span> DOWNLOAD EXCEL
                    </asp:LinkButton>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownloadINV035" />
            <asp:PostBackTrigger ControlID="btnDownloadCRDSI801" />
            <asp:PostBackTrigger ControlID="btnDownloadRDSMEST" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>