<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Unix_Web.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Trang chủ</title>
    <link href="https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Outfit', sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .dashboard-container {
            background: white;
            border-radius: 24px;
            padding: 50px;
            max-width: 600px;
            width: 100%;
            box-shadow: 0 30px 80px rgba(0, 78, 137, 0.12);
            text-align: center;
        }

        .success-icon {
            width: 80px;
            height: 80px;
            background: linear-gradient(135deg, #FF6B35 0%, #E55A2B 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 30px;
        }

        .success-icon svg {
            width: 40px;
            height: 40px;
            stroke: white;
        }

        h1 {
            font-size: 32px;
            color: #1A1A2E;
            margin-bottom: 10px;
        }

        .welcome-text {
            font-size: 18px;
            color: #6B6B7C;
            margin-bottom: 30px;
        }

        .user-info {
            background: #F8F9FA;
            border-radius: 16px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: left;
        }

        .info-row {
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #E8E8ED;
        }

        .info-row:last-child {
            border-bottom: none;
        }

        .info-label {
            font-weight: 600;
            color: #1A1A2E;
        }

        .info-value {
            color: #6B6B7C;
        }

        .btn-logout {
            padding: 16px 40px;
            background: linear-gradient(135deg, #FF6B35 0%, #E55A2B 100%);
            color: white;
            border: none;
            border-radius: 12px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            font-family: 'Outfit', sans-serif;
            box-shadow: 0 8px 20px rgba(255, 107, 53, 0.3);
        }

        .btn-logout:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 28px rgba(255, 107, 53, 0.4);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="dashboard-container">
            <div class="success-icon">
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M20 6L9 17L4 12" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
            </div>
            
            <h1>Đăng nhập thành công!</h1>
            <p class="welcome-text">Chào mừng bạn đã quay trở lại</p>
            
            <div class="user-info">
                <div class="info-row">
                    <span class="info-label">Tên đăng nhập:</span>
                    <asp:Label ID="lblUsername" runat="server" CssClass="info-value"></asp:Label>
                </div>
                <div class="info-row">
                    <span class="info-label">Thời gian đăng nhập:</span>
                    <asp:Label ID="lblLoginTime" runat="server" CssClass="info-value"></asp:Label>
                </div>
                <div class="info-row">
                    <span class="info-label">Trạng thái:</span>
                    <span class="info-value" style="color: #10b981; font-weight: 600;">Đang hoạt động</span>
                </div>
            </div>
            
            <asp:Button ID="btnLogout" runat="server" Text="Đăng xuất" CssClass="btn-logout" OnClick="btnLogout_Click" />
        </div>
    </form>
</body>
</html>
