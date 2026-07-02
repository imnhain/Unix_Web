<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Unix_Web.Admin.UserManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Quản lý phân quyền
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .per-container {
            background: rgba(30, 41, 59, 0.6);
            backdrop-filter: blur(20px);
            border: 1px solid rgba(0, 212, 255, 0.2);
            border-radius: 16px;
            padding: 20px;
        }

        .toolbar {
            display: flex;
            gap: 8px;
            background: rgba(255, 255, 255, 0.05);
            padding: 8px;
            border-radius: 8px;
            border: 1px solid rgba(0, 212, 255, 0.2);
            align-items: center;
            margin-bottom: 20px;
        }

        .btn-action {
            padding: 6px 16px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            transition: all 0.3s;
            border: 1px solid transparent;
            height: 32px;
            font-family: 'Poppins', sans-serif;
        }

        .btn-add  { background: #064e3b; color: #a7f3d0; border-color: #10b981; }
        .btn-del  { background: #7f1d1d; color: #fecaca; border-color: #ef4444; }
        .btn-save { background: #16a34a; color: white;   border-color: #22c55e; }
        .btn-action:hover { filter: brightness(1.3); transform: translateY(-1px); }

        .add-form {
            background: rgba(0, 0, 0, 0.3);
            border: 1px solid rgba(0, 212, 255, 0.3);
            border-radius: 10px;
            padding: 16px;
            margin-bottom: 20px;
        }

        .add-form-title {
            color: var(--primary);
            font-weight: 700;
            font-size: 13px;
            margin-bottom: 12px;
            text-transform: uppercase;
        }

        .add-form-row {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr auto;
            gap: 10px;
            align-items: end;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .form-label {
            font-size: 11px;
            font-weight: 600;
            color: var(--text-dim);
            text-transform: uppercase;
        }

        .form-input, .form-select {
            padding: 6px 10px;
            background: rgba(15, 23, 42, 0.8);
            border: 1px solid rgba(0, 212, 255, 0.3);
            border-radius: 6px;
            color: var(--text-light);
            font-size: 13px;
            height: 32px;
            width: 100%;
        }

        .form-input:focus, .form-select:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 8px rgba(0, 212, 255, 0.3);
        }

        .perm-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13px;
        }

        .perm-grid th {
            background: rgba(0, 212, 255, 0.1);
            color: var(--primary);
            padding: 10px 12px;
            text-align: left;
            border-bottom: 2px solid rgba(0, 212, 255, 0.3);
            font-weight: 700;
            font-size: 12px;
            text-transform: uppercase;
        }

        .perm-grid td {
            padding: 8px 12px;
            border-bottom: 1px solid rgba(0, 212, 255, 0.1);
            color: var(--text-light);
            vertical-align: middle;
        }

        .perm-grid tr:hover td {
            background: rgba(0, 212, 255, 0.05);
        }

        .badge {
            padding: 2px 10px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .badge-full  { background: rgba(16, 185, 129, 0.2); color: #10b981; border: 1px solid #10b981; }
        .badge-view  { background: rgba(99, 102, 241, 0.2);  color: #818cf8; border: 1px solid #818cf8; }
        .badge-admin { background: rgba(245, 158, 11, 0.2);  color: #f59e0b; border: 1px solid #f59e0b; }

        .count-badge {
            background: rgba(0, 212, 255, 0.1);
            color: var(--primary);
            padding: 2px 10px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 700;
            margin-left: 6px;
        }

        .empty-state {
            text-align: center;
            padding: 40px;
            color: var(--text-dim);
            font-size: 13px;
        }

        .alert {
            padding: 10px 16px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            margin-bottom: 15px;
        }

        .alert-success { background: rgba(16, 185, 129, 0.2); color: #10b981; border: 1px solid #10b981; }
        .alert-error   { background: rgba(239, 68, 68, 0.2);  color: #ef4444; border: 1px solid #ef4444; }
        .alert-warning { background: rgba(245, 158, 11, 0.2); color: #f59e0b; border: 1px solid #f59e0b; }

        .btn-del-inline {
            padding: 3px 10px;
            border-radius: 4px;
            font-size: 11px;
            font-weight: 600;
            cursor: pointer;
            background: rgba(127, 29, 29, 0.5);
            color: #fecaca;
            border: 1px solid #ef4444;
            transition: all 0.2s;
            font-family: 'Poppins', sans-serif;
        }

        .btn-del-inline:hover { background: #ef4444; color: white; }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="per-container">

        <%-- THÔNG BÁO --%>
        <asp:Label ID="lblNotify" runat="server" Visible="false"></asp:Label>

        <%-- TOOLBAR --%>
        <div class="toolbar">
            <asp:Button ID="btnShowAdd" runat="server" CssClass="btn-action btn-add"
                Text="➕ THÊM QUYỀN" OnClick="btnShowAdd_Click" />
            <span style="color: var(--text-dim); font-size: 12px; margin-left: 8px;">
                Tổng:
                <asp:Label ID="lblTotal" runat="server" CssClass="count-badge" Text="0"></asp:Label>
            </span>
        </div>

        <%-- FORM THÊM MỚI - ẨN/HIỆN QUA VISIBLE --%>
        <asp:Panel ID="pnlAddForm" runat="server" Visible="false">
            <div class="add-form">
                <div class="add-form-title">➕ Thêm quyền mới</div>
                <div class="add-form-row">
                    <div class="form-group">
                        <label class="form-label">Chương trình *</label>
                        <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-select">
                            <asp:ListItem Value="CPRDV01">CPRDV01</asp:ListItem>
                            <asp:ListItem Value="CPRDV02">CPRDV02</asp:ListItem>
                            <asp:ListItem Value="DOWNLOAD">DOWNLOAD</asp:ListItem>
                            <asp:ListItem Value="MONTHLOCK">MONTHLOCK</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Username *</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input"
                            placeholder="VD: nhanit" MaxLength="50"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Quyền *</label>
                        <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select">
                            <asp:ListItem Value="FULL">FULL - Toàn quyền</asp:ListItem>
                            <asp:ListItem Value="VIEW">VIEW - Chỉ xem</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div style="display:flex; gap:8px;">
                        <asp:Button ID="btnSave" runat="server" CssClass="btn-action btn-save"
                            Text="✓ LƯU" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancelAdd" runat="server" CssClass="btn-action btn-del"
                            Text="✕ HUỶ" OnClick="btnCancelAdd_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <%-- DANH SÁCH QUYỀN --%>
        <table class="perm-grid">
            <thead>
                <tr>
                    <th style="width:50px;">#</th>
                    <th style="width:200px;">Chương trình</th>
                    <th>Username</th>
                    <th style="width:150px;">Quyền</th>
                    <th style="width:100px;">Thao tác</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptPermissions" runat="server" OnItemCommand="rptPermissions_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td style="color: var(--text-dim);"><%# Container.ItemIndex + 1 %></td>
                            <td style="color: var(--primary); font-weight:600; font-family:'Courier New',monospace;">
                                <%# Eval("Program") %>
                            </td>
                            <td><%# Eval("Username") %></td>
                            <td>
                                <span class='badge <%# Eval("Role").ToString().ToUpper() == "FULL" ? "badge-full" : Eval("Role").ToString().ToUpper() == "ADMIN" ? "badge-admin" : "badge-view" %>'>
                                    <%# Eval("Role") %>
                                </span>
                            </td>
                            <td>
                                <asp:Button runat="server"
                                    CssClass="btn-del-inline"
                                    CommandName="Delete"
                                    CommandArgument='<%# Eval("Program") + "|" + Eval("Username") %>'
                                    Text="🗑️ XOÁ"
                                    OnClientClick="return confirm('Xoá quyền này?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:PlaceHolder runat="server" Visible='<%# rptPermissions.Items.Count == 0 %>'>
                            <tr>
                                <td colspan="5" class="empty-state">Chưa có quyền nào được cấu hình</td>
                            </tr>
                        </asp:PlaceHolder>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>

</asp:Content>
