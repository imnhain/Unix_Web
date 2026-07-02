<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" CodeBehind="cprdv01.aspx.cs" Inherits="Unix_Web.Source.CPRD.cprdv01" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    KV2 - Thông số kiểm tra (CPRDV01)
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

        .toolbar-wrapper {
            margin-bottom: 15px;
            min-height: 40px;
        }

        .toolbar {
            display: flex;
            gap: 8px;
            background: rgba(255, 255, 255, 0.05);
            padding: 8px;
            border-radius: 8px;
            border: 1px solid rgba(0, 212, 255, 0.2);
            align-items: center;
        }

        .btn-action {
            padding: 6px 16px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            text-decoration: none;
            transition: all 0.3s;
            border: 1px solid transparent;
            height: 32px;
            white-space: nowrap;
        }

        .btn-query { background: #1e3a8a; color: #bfdbfe; border-color: #3b82f6; }
        .btn-add { background: #064e3b; color: #a7f3d0; border-color: #10b981; }
        .btn-mod { background: #78350f; color: #fde68a; border-color: #f59e0b; }
        .btn-del { background: #7f1d1d; color: #fecaca; border-color: #ef4444; }
        .btn-excel { background: #14532d; color: #bbf7d0; border-color: #22c55e; }
        .btn-ok { background: #16a34a; color: white; border-color: #22c55e; width: 90px; }
        .btn-cancel { background: #dc2626; color: white; border-color: #ef4444; width: auto; min-width: 90px; }
        .btn-fetch { background: #4338ca; color: #c7d2fe; border-color: #6366f1; min-width: 40px; }

        .btn-action:hover { filter: brightness(1.3); transform: translateY(-2px); }
        .btn-action[disabled] { opacity: 0.4; cursor: not-allowed; filter: grayscale(100%); }

        .action-status {
            margin-left: auto;
            font-weight: bold;
            color: var(--accent);
            text-transform: uppercase;
            font-size: 12px;
            animation: blink 1.5s infinite;
        }

        @keyframes blink { 50% { opacity: 0.5; } }

        /* Two column layout for GROUP1 and GROUP2 */
        .two-col-layout {
            display: grid;
            grid-template-columns: 55fr 45fr;
            gap: 15px;
            margin-bottom: 12px;
        }

        /* Group Box */
        .per-group {
            background: rgba(0, 0, 0, 0.2);
            border: 1px solid rgba(0, 212, 255, 0.3);
            border-radius: 10px;
            padding: 12px;
        }

        .per-group legend {
            color: var(--primary);
            font-weight: 700;
            font-size: 13px;
            padding: 0 10px;
            letter-spacing: 0.5px;
        }

        /* Field styles - Fixed width labels for vertical alignment */
        .field-label {
            font-size: 10px;
            font-weight: 600;
            color: var(--text-dim);
            text-align: right;
            display: inline-block;
            margin-right: 4px;
            width: 85px; /* Fixed width for all labels */
        }

        .field-label-short {
            width: 85px; /* Same as normal - for consistency */
        }

        .field-label-tiny {
            width: 50px; /* For very short labels like subno */
        }

        .field-input {
            padding: 2px 4px;
            background: rgba(15, 23, 42, 0.8);
            border: 1px solid rgba(0, 212, 255, 0.3);
            border-radius: 3px;
            color: var(--text-light);
            font-size: 11px;
            font-family: 'Courier New', monospace;
            height: 20px;
        }

        .field-input:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 8px rgba(0, 212, 255, 0.3);
        }

        .field-input:read-only {
            background: rgba(15, 23, 42, 0.5);
            color: rgba(226, 232, 240, 0.6);
            cursor: not-allowed;
        }

        /* Specific widths */
        .w-30 { width: 30px; }
        .w-40 { width: 40px; }
        .w-50 { width: 50px; }
        .w-60 { width: 60px; }
        .w-80 { width: 80px; }
        .w-100 { width: 100px; }
        .w-150 { width: 150px; }
        .w-200 { width: 200px; }
        .w-250 { width: 250px; }
        .w-300 { width: 300px; }

        /* Color classes */
        .color-yellow { background: #fef08a !important; color: #000 !important; }
        .color-red { background: #fca5a5 !important; color: #000 !important; }
        .color-cyan { background: #67e8f9 !important; color: #000 !important; }
        .color-green { background: #86efac !important; color: #000 !important; }
        .color-magenta { background: #f0abfc !important; color: #000 !important; }

        /* Grid layouts */
        .grid-line {
            margin-bottom: 4px;
            display: flex;
            align-items: center;
            gap: 4px;
        }

        /* Status line */
        .status-line {
            display: flex;
            justify-content: center;
            padding: 6px 12px;
            background: rgba(0, 0, 0, 0.3);
            border-radius: 6px;
            font-size: 11px;
            color: var(--text-dim);
            border: 1px solid rgba(0, 212, 255, 0.2);
            margin-top: 8px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="updMain" runat="server">
        <ContentTemplate>
        <div class="per-container">
            <asp:Panel ID="pnlMainToolbar" runat="server" CssClass="toolbar-wrapper">
                <div class="toolbar">
                    <asp:LinkButton ID="btnQuery" runat="server" CssClass="btn-action btn-query" OnClick="btnQuery_Click">
                        <span>🔍</span> QUERY
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn-action btn-add" OnClick="btnAdd_Click">
                        <span>➕</span> ADD
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnModify" runat="server" CssClass="btn-action btn-mod" OnClick="btnModify_Click">
                        <span>✏️</span> MODIFY
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-action btn-del" OnClick="btnDelete_Click">
                        <span>🗑️</span> DELETE
                    </asp:LinkButton>
                    <asp:HiddenField ID="hdnDeleteConfirmed" runat="server" Value="false" />
                    <asp:LinkButton ID="btnExcelAll" runat="server" CssClass="btn-action btn-excel" OnClick="btnExcelAll_Click">
                        <span>📥</span> EXCEL ALL
                    </asp:LinkButton>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlConfirmToolbar" runat="server" CssClass="toolbar-wrapper" Visible="false">
                <div class="toolbar">
                    <asp:LinkButton ID="btnOK" runat="server" CssClass="btn-action btn-ok" OnClick="btnOK_Click"
                        OnClientClick="if(typeof(Page_ClientValidate) == 'function') Page_ClientValidate();">
                        ✓ OK
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn-action btn-cancel" OnClick="btnCancel_Click" >
                        ✕ CANCEL
                    </asp:LinkButton>
                    <asp:Label ID="lblActionStatus" runat="server" CssClass="action-status"></asp:Label> 
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlNavigationToolbar" runat="server" CssClass="toolbar-wrapper" Visible="false">
                <div class="toolbar">
                    <asp:LinkButton ID="btnFetchFirst" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchFirst_Click">
                        ⏮️ FIRST
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnFetchPrevious" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchPrevious_Click">
                        ◀️ PREV
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnFetchNext" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchNext_Click">
                        NEXT ▶️
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnFetchLast" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchLast_Click">
                        LAST ⏭️
                    </asp:LinkButton>
                    <span style="margin-left: 10px; color: var(--text-dim); font-size: 12px;">
                        Record: <asp:Label ID="lblNavIndex" runat="server" Text="0" style="color: var(--primary); font-weight: bold;"></asp:Label> / 
                        <asp:Label ID="lblNavCount" runat="server" Text="0" style="color: var(--primary); font-weight: bold;"></asp:Label>
                    </span>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlInput" runat="server">
                
                <div class="two-col-layout">
                    
                    <fieldset class="per-group">
                        <legend>GROUP01</legend>
                        
                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_subno">subno</span>
                            <asp:TextBox ID="txtSubno" runat="server" CssClass="field-input color-yellow w-40" ReadOnly="true"></asp:TextBox>
                            
                            <span class="field-label" style="width: 50px;" data-i18n="cprdv01_factory">factory</span>
                            <asp:TextBox ID="txtFactory" runat="server" CssClass="field-input color-yellow w-30" ReadOnly="true"></asp:TextBox>
                            
                            <span class="field-label" style="width: 50px;" data-i18n="cprdv01_itnbr">itnbr</span>
                            <asp:TextBox ID="txtItnbr" runat="server" CssClass="field-input color-yellow w-150" MaxLength="8" 
                                       AutoPostBack="true" OnTextChanged="txtItnbr_TextChanged"></asp:TextBox>
                            
                            <span class="field-label" style="width: 60px;" data-i18n="cprdv01_version">VERSION</span>
                            <asp:TextBox ID="txtVersion" runat="server" CssClass="field-input color-red w-50" ReadOnly="true"></asp:TextBox>
                            
                            <span class="field-label" style="width: 50px;" data-i18n="cprdv01_stype">STYPE</span>
                            <asp:TextBox ID="txtStype" runat="server" CssClass="field-input color-yellow w-30" MaxLength="1"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machno">machno</span>
                            <asp:TextBox ID="txtMachno" runat="server" CssClass="field-input w-100" MaxLength="5"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm1">machm1</span>
                            <asp:TextBox ID="txtMachm1" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs1">machs1</span>
                            <asp:TextBox ID="txtMachs1" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm2">machm2</span>
                            <asp:TextBox ID="txtMachm2" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs2">machs2</span>
                            <asp:TextBox ID="txtMachs2" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm3">machm3</span>
                            <asp:TextBox ID="txtMachm3" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs3">machs3</span>
                            <asp:TextBox ID="txtMachs3" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm4">machm4</span>
                            <asp:TextBox ID="txtMachm4" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs4">machs4</span>
                            <asp:TextBox ID="txtMachs4" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm5">machm5</span>
                            <asp:TextBox ID="txtMachm5" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs5">machs5</span>
                            <asp:TextBox ID="txtMachs5" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm6">machm6</span>
                            <asp:TextBox ID="txtMachm6" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs6">machs6</span>
                            <asp:TextBox ID="txtMachs6" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm7">machm7</span>
                            <asp:TextBox ID="txtMachm7" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs7">machs7</span>
                            <asp:TextBox ID="txtMachs7" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm8">machm8</span>
                            <asp:TextBox ID="txtMachm8" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs8">machs8</span>
                            <asp:TextBox ID="txtMachs8" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm9">machm9</span>
                            <asp:TextBox ID="txtMachm9" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs9">machs9</span>
                            <asp:TextBox ID="txtMachs9" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm10">machm10</span>
                            <asp:TextBox ID="txtMachm10" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs10">machs10</span>
                            <asp:TextBox ID="txtMachs10" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm11">machm11</span>
                            <asp:TextBox ID="txtMachm11" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs11">machs11</span>
                            <asp:TextBox ID="txtMachs11" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm12">machm12</span>
                            <asp:TextBox ID="txtMachm12" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs12">machs12</span>
                            <asp:TextBox ID="txtMachs12" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm13">machm13</span>
                            <asp:TextBox ID="txtMachm13" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs13">machs13</span>
                            <asp:TextBox ID="txtMachs13" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm14">machm14</span>
                            <asp:TextBox ID="txtMachm14" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs14">machs14</span>
                            <asp:TextBox ID="txtMachs14" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_machm15">machm15</span>
                            <asp:TextBox ID="txtMachm15" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs15">machs15</span>
                            <asp:TextBox ID="txtMachs15" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv01_machm16">machm16</span>
                            <asp:TextBox ID="txtMachm16" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_machs16">machs16</span>
                            <asp:TextBox ID="txtMachs16" runat="server" CssClass="field-input color-cyan w-40" MaxLength="2"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label" data-i18n="cprdv01_totalm">totalm</span>
                            <asp:TextBox ID="txtTotalm" runat="server" CssClass="field-input color-cyan w-40" ReadOnly="true"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_totals">totals</span>
                            <asp:TextBox ID="txtTotals" runat="server" CssClass="field-input color-cyan w-40" ReadOnly="true"></asp:TextBox>
                        </div>
                    </fieldset>

                    <fieldset class="per-group">
                        <legend>GROUP02</legend>
                        
                        <div class="grid-line">
                            <span class="field-label field-label-short" data-i18n="cprdv01_check1">check1</span>
                            <asp:TextBox ID="txtCheck1" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check2">check2</span>
                            <asp:TextBox ID="txtCheck2" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check3">check3</span>
                            <asp:TextBox ID="txtCheck3" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check4">check4</span>
                            <asp:TextBox ID="txtCheck4" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label field-label-short" data-i18n="cprdv01_check5">check5</span>
                            <asp:TextBox ID="txtCheck5" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check6">check6</span>
                            <asp:TextBox ID="txtCheck6" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check7">check7</span>
                            <asp:TextBox ID="txtCheck7" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check8">check8</span>
                            <asp:TextBox ID="txtCheck8" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label field-label-short" data-i18n="cprdv01_check9">check9</span>
                            <asp:TextBox ID="txtCheck9" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check10">check10</span>
                            <asp:TextBox ID="txtCheck10" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check11">check11</span>
                            <asp:TextBox ID="txtCheck11" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check12">check12</span>
                            <asp:TextBox ID="txtCheck12" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                        </div>

                        <div class="grid-line">
                            <span class="field-label field-label-short" data-i18n="cprdv01_check13">check13</span>
                            <asp:TextBox ID="txtCheck13" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check14">check14</span>
                            <asp:TextBox ID="txtCheck14" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check15">check15</span>
                            <asp:TextBox ID="txtCheck15" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                            <span class="field-label field-label-short" data-i18n="cprdv01_check16">check16</span>
                            <asp:TextBox ID="txtCheck16" runat="server" CssClass="field-input color-green w-50" MaxLength="3"></asp:TextBox>
                        </div>
                    </fieldset>
                </div>

                <fieldset class="per-group">
                    <legend>GROUP03</legend>
                    
                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_ostoptime">ostoptime</span>
                        <asp:TextBox ID="txtOstoptime" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_tstoptime">stoptime</span>
                        <asp:TextBox ID="txtTstoptime" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_oopentime">oopentime</span>
                        <asp:TextBox ID="txtOopentime" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_topentime">topentime</span>
                        <asp:TextBox ID="txtTopentime" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                    </div>

                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_opencheck">opencheck</span>
                        <asp:TextBox ID="txtOpencheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_oplencheck">oplencheck</span>
                        <asp:TextBox ID="txtOplencheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_cllencheck">cllencheck</span>
                        <asp:TextBox ID="txtCllencheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_speedlen">speedlen</span>
                        <asp:TextBox ID="txtSpeedlen" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                    </div>

                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_opostopcheck">opostopcheck</span>
                        <asp:TextBox ID="txtOpostopcheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_optstopcheck">optstopcheck</span>
                        <asp:TextBox ID="txtOptstopcheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_clostopcheck">clostopcheck</span>
                        <asp:TextBox ID="txtClostopcheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_cltstopcheck">cltstopcheck</span>
                        <asp:TextBox ID="txtCltstopcheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                    </div>

                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_wopcheck">wopcheck</span>
                        <asp:TextBox ID="txtWopcheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_wclcheck">wclcheck</span>
                        <asp:TextBox ID="txtWclcheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_opchecklen">opchecklen</span>
                        <asp:TextBox ID="txtOpchecklen" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_inocheck">inocheck</span>
                        <asp:TextBox ID="txtInocheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_ouocheck">ouocheck</span>
                        <asp:TextBox ID="txtOuocheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_tocheck">tocheck</span>
                        <asp:TextBox ID="txtTocheck" runat="server" CssClass="field-input color-magenta w-80" MaxLength="3"></asp:TextBox>
                    </div>

                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_lheight">lheight</span>
                        <asp:TextBox ID="txtLheight" runat="server" CssClass="field-input color-magenta w-80" MaxLength="4"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_rheight">rheight</span>
                        <asp:TextBox ID="txtRheight" runat="server" CssClass="field-input color-magenta w-80" MaxLength="4"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_ltype">ltype</span>
                        <asp:TextBox ID="txtLtype" runat="server" CssClass="field-input color-magenta w-80" MaxLength="4"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_rtype">rtype</span>
                        <asp:TextBox ID="txtRtype" runat="server" CssClass="field-input color-magenta w-80" MaxLength="4"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_pressure">pressure</span>
                        <asp:TextBox ID="txtPressure" runat="server" CssClass="field-input color-magenta w-150"></asp:TextBox>
                    </div>

                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_lput">lput</span>
                        <asp:TextBox ID="txtLput" runat="server" CssClass="field-input color-magenta w-80" MaxLength="4"></asp:TextBox>
                        <span class="field-label" data-i18n="cprdv01_rput">rput</span>
                        <asp:TextBox ID="txtRput" runat="server" CssClass="field-input color-magenta w-80" MaxLength="4"></asp:TextBox>
                    </div>
                </fieldset>

                <fieldset class="per-group">
                    <legend>GROUP04</legend>
                    
                    <div class="grid-line">
                        <span class="field-label" data-i18n="cprdv01_itdsc">TEN HANG</span>
                        <asp:TextBox ID="txtItdsc" runat="server" CssClass="field-input color-yellow w-250" MaxLength="30"></asp:TextBox>
                        <asp:TextBox ID="txtSpdsc" runat="server" CssClass="field-input color-yellow w-300" MaxLength="30"></asp:TextBox>
                        
                        <span class="field-label" data-i18n="cprdv01_indat_label">NGAY NHAP</span>
                        <asp:Label ID="lblIndat" runat="server" CssClass="field-input w-100" style="border: none; background: transparent;"></asp:Label>
                        
                        <span class="field-label" data-i18n="cprdv01_userno_label">NGUOI NHAP</span>
                        <asp:Label ID="lblUserNo" runat="server" CssClass="field-input w-100" style="border: none; background: transparent;"></asp:Label>
                    </div>
                </fieldset>

                <div class="status-line">
                    <span>- <span data-i18n="cprdv01_rows">Rows</span> <asp:Label ID="lblIndex" runat="server" Text="0"></asp:Label>/<asp:Label ID="lblCount" runat="server" Text="0"></asp:Label> -</span>
                </div>
            </asp:Panel>

            <asp:HiddenField ID="hdnRowID" runat="server" />
        </div>
        </ContentTemplate>
            <Triggers>
            <asp:PostBackTrigger ControlID="btnExcelAll" />
        </Triggers>
    </asp:UpdatePanel> 

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            document.addEventListener('keydown', function (event) {
                var confirmToolbar = document.getElementById('<%= pnlConfirmToolbar.ClientID %>');
                var isConfirmMode = confirmToolbar && window.getComputedStyle(confirmToolbar).display !== 'none';
                
                if ((event.key === 'Escape' || event.keyCode === 27) && isConfirmMode) {
                    event.preventDefault();
                    event.stopPropagation();
                    
                    var btnOK = document.getElementById('<%= btnOK.ClientID %>');
                    if (btnOK && !btnOK.disabled) {
                        btnOK.click();
                    }
                    return false;
                }
            });
        });

        // ==========================================
        // 1. CHỨC NĂNG XỬ LÝ SỐ 0 (UX/UI CẢI THIỆN)
        // ==========================================
        function bindZeroHandling() {
            // Danh sách các tiền tố ID của các ô nhập số cần chức năng này
            const numericPrefixes = [
                'txtMachm', 'txtMachs', 'txtCheck', 'txtOstoptime', 'txtTstoptime',
                'txtOopentime', 'txtTopentime', 'txtOpencheck', 'txtOplencheck',
                'txtCllencheck', 'txtSpeedlen', 'txtOpostopcheck', 'txtOptstopcheck',
                'txtClostopcheck', 'txtCltstopcheck', 'txtWopcheck', 'txtWclcheck',
                'txtOpchecklen', 'txtInocheck', 'txtOuocheck', 'txtTocheck',
                'txtLheight', 'txtRheight', 'txtLtype', 'txtRtype', 'txtPressure',
                'txtLput', 'txtRput'
            ];

            // Tạo chuỗi selector CSS để lấy tất cả các ô trên
            let selector = numericPrefixes.map(prefix => "input[id*='" + prefix + "']").join(', ');
            var inputs = document.querySelectorAll(selector);

            inputs.forEach(function (input) {
                // Xóa sự kiện cũ (tránh gắn lặp khi UpdatePanel load lại)
                input.removeEventListener('focus', removeZero);
                input.removeEventListener('blur', addZeroIfEmpty);

                // Gắn sự kiện mới
                input.addEventListener('focus', removeZero);
                input.addEventListener('blur', addZeroIfEmpty);
            });
        }

        // Khi con trỏ chuột trỏ vào ô (Focus)
        function removeZero(e) {
            let val = e.target.value.trim();
            // Nếu giá trị đang là 0 thì xóa trắng đi để gõ thẳng số mới (VD: gõ 23 sẽ ra 23)
            if (val === '0' || val === '00') {
                e.target.value = '';
            }
        }

        // Khi con trỏ chuột rời khỏi ô (Blur)
        function addZeroIfEmpty(e) {
            let val = e.target.value.trim();
            // Nếu người dùng không nhập gì cả, tự động điền lại số 0
            if (val === '') {
                e.target.value = '0';
            } else {
                // Tùy chọn nâng cao: Xóa bỏ số 0 vô tình ở đầu (VD: người dùng tự gõ '023' -> sửa thành '23')
                if (!isNaN(val) && val !== '') {
                    e.target.value = Number(val).toString();
                }
            }
        }


        // ==========================================
        // 2. CHỨC NĂNG TÍNH TỔNG THỜI GIAN
        // ==========================================
        function bindTimeCalculations() {
            var inputs = document.querySelectorAll("input[id*='txtMachm'], input[id*='txtMachs']");

            inputs.forEach(function (input) {
                input.removeEventListener('blur', calculateTotalTimeClient);
                input.removeEventListener('change', calculateTotalTimeClient);

                input.addEventListener('blur', calculateTotalTimeClient);
                input.addEventListener('change', calculateTotalTimeClient);
            });
        }

        function calculateTotalTimeClient() {
            let totalM = 0;
            let totalS = 0;

            for (let i = 1; i <= 16; i++) {
                let txtM = document.querySelector(`input[id$='txtMachm${i}']`);
                let txtS = document.querySelector(`input[id$='txtMachs${i}']`);

                let valM = (txtM && txtM.value) ? parseInt(txtM.value, 10) : 0;
                let valS = (txtS && txtS.value) ? parseInt(txtS.value, 10) : 0;

                if (!isNaN(valM)) totalM += valM;
                if (!isNaN(valS)) totalS += valS;
            }

            totalM += Math.floor(totalS / 60);
            totalS = totalS % 60;

            let strM = totalM.toString().padStart(2, '0');
            let strS = totalS.toString().padStart(2, '0');

            let outM = document.querySelector(`input[id$='txtTotalm']`);
            let outS = document.querySelector(`input[id$='txtTotals']`);

            if (outM) outM.value = strM;
            if (outS) outS.value = strS;
        }


        // ==========================================
        // 3. KHỞI CHẠY TẤT CẢ
        // ==========================================
        function initAllScripts() {
            bindZeroHandling();
            bindTimeCalculations();
        }

        // Chạy lần đầu khi mở trang
        document.addEventListener("DOMContentLoaded", function () {
            initAllScripts();
        });

        // Chạy lại sau mỗi lần PostBack của ASP.NET UpdatePanel
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function (sender, e) {
                initAllScripts();
            });
        }
    </script>
</asp:Content>