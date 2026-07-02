<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" CodeBehind="cprdv02.aspx.cs" Inherits="Unix_Web.Source.CPRD.cprdv02" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    KV2 - Kế hoạch sản xuất (CPRDV02)
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
        .btn-ok { background: #16a34a; color: white; border-color: #22c55e; width: 90px; justify-content: center; }
        .btn-cancel { 
            background: #dc2626; 
            color: white; 
            border-color: #ef4444; 
            width: auto;
            min-width: 90px;
        }
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

        /* Main Grid Layout - 3 cột như file .per */
        .grid-main {
            display: grid;
            grid-template-columns: 320px 400px 340px;
            gap: 15px;
            margin-bottom: 15px;
        }

        /* Grid dưới - 2 cột */
        .grid-bottom {
            display: grid;
            grid-template-columns: 480px 480px;
            gap: 15px;
            margin-bottom: 15px;
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

        /* Field Row - căn chỉnh label bên trái */
        .field-row {
            display: grid;
            grid-template-columns: 90px 1fr;
            gap: 6px;
            align-items: center;
            margin-bottom: 6px;
        }

        .field-row-double {
            display: grid;
            grid-template-columns: 70px 1fr 70px 1fr;
            gap: 6px;
            align-items: center;
            margin-bottom: 6px;
        }

        .field-row-triple {
            display: grid;
            grid-template-columns: 55px 70px 55px 50px 60px 1fr;
            gap: 6px;
            align-items: center;
            margin-bottom: 6px;
        }

        .field-label {
            font-size: 11px;
            font-weight: 600;
            color: var(--text-dim);
            text-align: left;
            padding-left: 4px;
        }

        .field-input, .field-select {
            padding: 4px 6px;
            background: rgba(15, 23, 42, 0.8);
            border: 1px solid rgba(0, 212, 255, 0.3);
            border-radius: 4px;
            color: var(--text-light);
            font-size: 12px;
            font-family: 'Courier New', monospace;
            width: 100%;
            height: 24px;
        }

        .field-input:focus, .field-select:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 10px rgba(0, 212, 255, 0.3);
        }

        .field-input:read-only {
            background: rgba(15, 23, 42, 0.5);
            color: rgba(226, 232, 240, 0.6);
            cursor: not-allowed;
        }

        .field-select:disabled {
            background: rgba(15, 23, 42, 0.5);
            color: rgba(226, 232, 240, 0.6);
            cursor: not-allowed;
        }

        /* Color classes */
        .text-red { color: #fca5a5 !important; }
        .text-blue { color: #93c5fd !important; }

        /* Specific widths */
        .w-60 { max-width: 60px; }
        .w-80 { max-width: 80px; }
        .w-100 { max-width: 100px; }

        /* Status line */
        .status-line {
            display: flex;
            justify-content: space-between;
            padding: 8px 15px;
            background: rgba(0, 0, 0, 0.3);
            border-radius: 6px;
            font-size: 12px;
            color: var(--text-dim);
            border: 1px solid rgba(0, 212, 255, 0.2);
            margin-top: 10px;
        }

        /* Responsive */
        @media (max-width: 1400px) {
            .grid-main {
                grid-template-columns: 1fr;
            }
            .grid-bottom {
                grid-template-columns: 1fr;
            }
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
                    <asp:LinkButton ID="btnExcel" runat="server" CssClass="btn-action btn-excel" OnClick="btnExcel_Click">
                        <span>📊</span> EXCEL
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnExcelAll" runat="server" CssClass="btn-action btn-excel" OnClick="btnExcelAll_Click">
                        <span>📥</span> EXCEL ALL
                    </asp:LinkButton>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlConfirmToolbar" runat="server" CssClass="toolbar-wrapper" Visible="false">
                <div class="toolbar">
                    <asp:LinkButton ID="btnOK" runat="server" CssClass="btn-action btn-ok" OnClick="btnOK_Click">
                        ✓ OK
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn-action btn-cancel" OnClick="btnCancel_Click">
                        ✕ CANCEL
                    </asp:LinkButton>
                    <asp:Label ID="lblActionStatus" runat="server" CssClass="action-status"></asp:Label>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlNavigationToolbar" runat="server" CssClass="toolbar-wrapper" Visible="false">
                <div class="toolbar">
                    <asp:LinkButton ID="btnFetchFirst" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchFirst_Click" ToolTip="First Record">
                        ⏮️ FIRST
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnFetchPrevious" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchPrevious_Click" ToolTip="Previous Record">
                        ◀️ PREV
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnFetchNext" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchNext_Click" ToolTip="Next Record">
                        NEXT ▶️
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnFetchLast" runat="server" CssClass="btn-action btn-fetch" OnClick="btnFetchLast_Click" ToolTip="Last Record">
                        LAST ⏭️
                    </asp:LinkButton>
                    <span style="margin-left: 10px; color: var(--text-dim); font-size: 12px;">
                        Record: <asp:Label ID="lblNavIndex" runat="server" Text="0" style="color: var(--primary); font-weight: bold;"></asp:Label> / 
                        <asp:Label ID="lblNavCount" runat="server" Text="0" style="color: var(--primary); font-weight: bold;"></asp:Label>
                    </span>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlInput" runat="server">
                <div class="grid-main">
                    <fieldset class="per-group">
                        <legend>GROUP2</legend>
                    
                        <div class="field-row">
                            <span class="field-label text-red" data-i18n="cprdv02_machno">MACHNO *</span>
                            <asp:TextBox ID="txtMachNo" runat="server" CssClass="field-input" MaxLength="3"
                                       AutoPostBack="true" OnTextChanged="txtMachNo_TextChanged"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label text-blue" data-i18n="cprdv02_itnbr">ITNBR</span>
                            <asp:TextBox ID="txtItnbr" runat="server" CssClass="field-input" MaxLength="8"
                                       AutoPostBack="true" OnTextChanged="txtItnbr_TextChanged"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_lmoldsize">LMOLDSIZE</span>
                            <asp:TextBox ID="txtLMoldSize" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_rmoldsize">RMOLDSIZE</span>
                            <asp:TextBox ID="txtRMoldSize" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_tireno">TIRENO</span>
                            <asp:TextBox ID="txtTireNo2" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_ringno">RINGNO</span>
                            <asp:TextBox ID="txtRingNo" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_batchclamp">BATCHCLAMP</span>
                            <asp:TextBox ID="txtBatchClamp" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    </fieldset>

                    <fieldset class="per-group">
                        <legend>GROUP3</legend>
                    
                        <div class="field-row-triple">
                            <span class="field-label" data-i18n="cprdv02_specpci">SPECPCI</span>
                            <asp:TextBox ID="txtSpecPci" runat="server" CssClass="field-input" ReadOnly="true"></asp:TextBox>
                            <span class="field-label text-red" data-i18n="cprdv02_version">VERSION</span>
                            <asp:TextBox ID="txtVersion" runat="server" CssClass="field-input w-60" ReadOnly="true"></asp:TextBox>
                            <span class="field-label text-blue" data-i18n="cprdv02_stype">STYPE</span>
                            <asp:TextBox ID="txtStype" runat="server" CssClass="field-input w-60" ReadOnly="true"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_lbatchsize">LBATCHSIZE</span>
                            <asp:TextBox ID="txtLBatchSize" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_rbatchsize">RBATCHSIZE</span>
                            <asp:TextBox ID="txtRBatchSize" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_color">COLOR</span>
                            <asp:DropDownList ID="ddlColor" runat="server" CssClass="field-select"></asp:DropDownList>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_color1">COLOR1</span>
                            <asp:DropDownList ID="ddlColor1" runat="server" CssClass="field-select"></asp:DropDownList>
                            <span class="field-label" data-i18n="cprdv02_color2">COLOR2</span>
                            <asp:DropDownList ID="ddlColor2" runat="server" CssClass="field-select"></asp:DropDownList>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_color3">COLOR3</span>
                            <asp:DropDownList ID="ddlColor3" runat="server" CssClass="field-select"></asp:DropDownList>
                            <span class="field-label" data-i18n="cprdv02_circolor">CIRCOLOR</span>
                            <asp:DropDownList ID="ddlCirColor" runat="server" CssClass="field-select"></asp:DropDownList>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_moldstyle">MOLDSTYLE</span>
                            <asp:TextBox ID="txtMoldStyle" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    </fieldset>

                    <fieldset class="per-group">
                        <legend>GROUP4</legend>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_tread">TREAD</span>
                            <asp:TextBox ID="txtTread" runat="server" CssClass="field-input"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv02_sidewall">SIDEWALL</span>
                            <asp:TextBox ID="txtSidewall" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_speed">SPEED</span>
                            <asp:TextBox ID="txtSpeed" runat="server" CssClass="field-input"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv02_structcod">STRUCTCOD</span>
                            <asp:TextBox ID="txtStructCod" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_dot">DOT</span>
                            <asp:TextBox ID="txtDot" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_maxload">MAXLOAD</span>
                            <asp:TextBox ID="txtMaxLoad" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_note1">NOTE1</span>
                            <asp:TextBox ID="txtNote1" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_note2">NOTE2</span>
                            <asp:TextBox ID="txtNote2" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    </fieldset>
                </div>

                <div class="grid-bottom">
                    <fieldset class="per-group">
                        <legend>GROUP5</legend>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_scansta">SCANSTA</span>
                            <asp:TextBox ID="txtScanSta" runat="server" CssClass="field-input w-100"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_force">FORCE</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtForce" runat="server" CssClass="field-input w-100"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px; white-space: nowrap;">±50</span>
                            </div>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_forcekn">FORCEKN</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtForceKn" runat="server" CssClass="field-input w-100"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px; white-space: nowrap;">±50</span>
                            </div>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_heightpci">HEIGHTPCI</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtHeightPci" runat="server" CssClass="field-input w-100"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px; white-space: nowrap;">±1</span>
                            </div>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_timepci">TIMEPCI</span>
                            <asp:TextBox ID="txtTimePci" runat="server" CssClass="field-input w-100"></asp:TextBox>
                        </div>
                    
                        <div class="field-row">
                            <span class="field-label" data-i18n="cprdv02_pressure">PRESSURE</span>
                            <asp:TextBox ID="txtPressure" runat="server" CssClass="field-input w-100"></asp:TextBox>
                        </div>
                    </fieldset>

                    <fieldset class="per-group">
                        <legend>GROUP6</legend>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_ltemp">LTEMP</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtLTemp" runat="server" CssClass="field-input"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px;">±5</span>
                            </div>
                            <span class="field-label" data-i18n="cprdv02_rtemp">RTEMP</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtRTemp" runat="server" CssClass="field-input"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px;">±5</span>
                            </div>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_leptemp">LEPTEMP</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtLEpTemp" runat="server" CssClass="field-input"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px;">±5</span>
                            </div>
                            <span class="field-label" data-i18n="cprdv02_reptemp">REPTEMP</span>
                            <div style="display: flex; align-items: center; gap: 5px;">
                                <asp:TextBox ID="txtREpTemp" runat="server" CssClass="field-input"></asp:TextBox>
                                <span style="color: var(--text-dim); font-size: 11px;">±5</span>
                            </div>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_lheight">LHEIGHT</span>
                            <asp:TextBox ID="txtLHeight" runat="server" CssClass="field-input"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv02_rheight">RHEIGHT</span>
                            <asp:TextBox ID="txtRHeight" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_ltype">LTYPE</span>
                            <asp:TextBox ID="txtLType" runat="server" CssClass="field-input"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv02_rtype">RTYPE</span>
                            <asp:TextBox ID="txtRType" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    
                        <div class="field-row-double">
                            <span class="field-label" data-i18n="cprdv02_lput">LPUT</span>
                            <asp:TextBox ID="txtLPut" runat="server" CssClass="field-input"></asp:TextBox>
                            <span class="field-label" data-i18n="cprdv02_rput">RPUT</span>
                            <asp:TextBox ID="txtRPut" runat="server" CssClass="field-input"></asp:TextBox>
                        </div>
                    </fieldset>
                </div>

                <div class="status-line">
                    <span>
                        <span data-i18n="cprdv02_rows">Rows:</span> 
                        <asp:Label ID="lblIndex" runat="server" Text="0"></asp:Label> / 
                        <asp:Label ID="lblCount" runat="server" Text="0"></asp:Label>
                    </span>
                    <span>
                        <span data-i18n="cprdv02_indat">Indat:</span> 
                        <asp:Label ID="lblIndat" runat="server"></asp:Label>
                    </span>
                    <span>
                        <span data-i18n="cprdv02_user">User:</span> 
                        <asp:Label ID="lblUserNo" runat="server"></asp:Label>
                    </span>
                </div>
            </asp:Panel>

            <asp:HiddenField ID="hdnRowID" runat="server" />
        </div>
    </ContentTemplate>
        <Triggers>
        <asp:PostBackTrigger ControlID="btnExcel" />
        <asp:PostBackTrigger ControlID="btnExcelAll" />
    </Triggers>
</asp:UpdatePanel>
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {

            document.addEventListener('keydown', function (event) {
                var confirmToolbar = document.getElementById('<%= pnlConfirmToolbar.ClientID %>');
            var isConfirmMode = confirmToolbar && window.getComputedStyle(confirmToolbar).display !== 'none';
            
            // ESC → OK
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
    </script>
</asp:Content>