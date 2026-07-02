<%@ Page Title="Khoá Tháng" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" CodeBehind="MonthLock.aspx.cs" Inherits="Unix_Web.Source.PRD.MonthLock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Quản lý Khóa Tháng (PRDSYSN)
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .form-card {
            background: rgba(30, 41, 59, 0.5);
            border: 1px solid var(--border-color);
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
        }
        .input-group {
            display: flex;
            gap: 15px;
            margin-bottom: 15px;
            align-items: center;
        }
        .form-control {
            background: var(--bg-dark);
            border: 1px solid var(--border-color);
            color: white;
            padding: 10px 15px;
            border-radius: 8px;
            outline: none;
        }
        .form-control:focus { border-color: var(--primary); }
        .btn {
            padding: 10px 20px;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            font-weight: 600;
            color: white;
            transition: all 0.3s;
        }
        .btn-blue { background: var(--secondary); }
        .btn-blue:hover { background: #4f46e5; }
        .btn-red { background: #ef4444; }
        .btn-red:hover { background: #dc2626; }
        .btn-green { background: #10b981; }
        .btn-green:hover { background: #059669; }
        
        /* Style cho GridView */
        .grid-container { overflow-x: auto; }
        .custom-grid { width: 100%; border-collapse: collapse; color: white; font-size: 13px;}
        .custom-grid th { background: var(--sidebar-bg); padding: 12px; border: 1px solid var(--border-color); }
        .custom-grid td { padding: 10px; border: 1px solid var(--border-color); text-align: center; }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-card">
        <div class="input-group">
            <label>DEPNO:</label>
            <asp:TextBox ID="txtDepno" runat="server" CssClass="form-control" placeholder="VD: P1300, P1500..."></asp:TextBox>
            
            <label>PRGNO:</label>
            <asp:TextBox ID="txtPrgno" runat="server" CssClass="form-control" placeholder="VD: PRDA01, PRDA11..."></asp:TextBox>

            <label>Tháng C.Khóa:</label>
            <asp:TextBox ID="txtlmonth" runat="server" CssClass="form-control" Width="100px" style="text-align:center; font-weight:bold;"></asp:TextBox>
        </div>

        <div class="input-group" style="flex-wrap: wrap;">
            <asp:LinkButton ID="btnSelect" runat="server" CssClass="btn btn-blue btn-action" OnClick="btnSelect_Click">🔍 Tìm kiếm</asp:LinkButton>
            <asp:LinkButton ID="btnSelect_All" runat="server" CssClass="btn btn-blue btn-action" OnClick="btnSelect_All_Click">📑 Load All</asp:LinkButton>
            <asp:LinkButton ID="btnKhoa" runat="server" CssClass="btn btn-red btn-action" OnClick="btnKhoa_Click">🔒 Khóa (Các dòng đã tick)</asp:LinkButton>
            <asp:LinkButton ID="btnMoKhoa" runat="server" CssClass="btn btn-green btn-action" OnClick="btnMoKhoa_Click">🔓 Mở Khóa (Các dòng đã tick)</asp:LinkButton>
            <asp:LinkButton ID="btnUpPRDA01" runat="server" CssClass="btn btn-blue btn-action" OnClick="btnUpPRDA01_Click">⚡ Update PRDA01</asp:LinkButton>
            <asp:LinkButton ID="btnUpPRDA11" runat="server" CssClass="btn btn-blue btn-action" OnClick="btnUpPRDA11_Click">⚡ Update PRDA11</asp:LinkButton>
            <asp:LinkButton ID="btn_maspakh" runat="server" CssClass="btn btn-blue btn-action" OnClick="btn_maspakh_Click">📦 Cập nhật MASPAKH</asp:LinkButton>
            <asp:LinkButton ID="btnPRD923" runat="server" CssClass="btn btn-green btn-action" OnClick="btnPRD923_Click">👥 PRD923</asp:LinkButton>
            <asp:LinkButton ID="btnCheckMaterialQty" runat="server" CssClass="btn btn-green btn-action" OnClick="btnCheckMaterialQty_Click">✅ Check PRDA07</asp:LinkButton>
        </div>

        <div class="input-group" style="margin-top: 15px; padding-top: 15px; border-top: 1px dashed rgba(255,255,255,0.1);">
            <label>Chọn loại QC (PRI):</label>
            <asp:DropDownList ID="ddlPRI_QC" runat="server" CssClass="form-control">
                <asp:ListItem Value="P5720">QC IC</asp:ListItem>
                <asp:ListItem Value="P8830">QC PCR</asp:ListItem>
            </asp:DropDownList>
            <asp:LinkButton ID="btnPRI_QC" runat="server" CssClass="btn btn-red btn-action" OnClick="btnPRI_QC_Click">⚙️ Cập nhật QC</asp:LinkButton>
        </div>
    </div>

    <div class="form-card grid-container">
        <asp:GridView ID="dataGridView1" runat="server" CssClass="custom-grid" 
            AutoGenerateColumns="false" DataKeyNames="prgno,depno,lmonth"
            AllowSorting="true" OnSorting="dataGridView1_Sorting">
            <Columns>
                <asp:TemplateField ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <input type="checkbox" id="chkAll" onclick="toggleAllChecks(this);" style="transform: scale(1.5); cursor: pointer;" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server" CssClass="row-checkbox" style="transform: scale(1.5); cursor: pointer;" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="prgno" HeaderText="PRGNO" SortExpression="prgno" />
                <asp:BoundField DataField="depno" HeaderText="DEPNO" SortExpression="depno" />
                <asp:BoundField DataField="lmonth" HeaderText="Tháng Khóa" SortExpression="lmonth" />
                <asp:BoundField DataField="indat" HeaderText="Ngày Cập Nhật" SortExpression="indat" />
                <asp:BoundField DataField="intime" HeaderText="Giờ Cập Nhật" SortExpression="intime" />
                <asp:BoundField DataField="usrno" HeaderText="Người Cập Nhật" SortExpression="usrno" />
            </Columns>
        </asp:GridView>
    </div>

    <script>
        function toggleAllChecks(source) {
            // Tìm tất cả các checkbox có class 'row-checkbox' bên trong GridView
            var checkboxes = document.querySelectorAll('.row-checkbox input[type="checkbox"]');
            for (var i = 0; i < checkboxes.length; i++) {
                checkboxes[i].checked = source.checked;
            }
        }
    </script>
</asp:Content>