<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Unix_Web.Auth.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Unix System Login</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800&family=Orbitron:wght@700;900&display=swap" rel="stylesheet" />
    
    <style>
        :root {
            --primary: #00d4ff;
            --primary-dark: #00a8cc;
            --secondary: #6366f1;
            --accent: #f59e0b;
            --bg-dark: #0f172a;
            --bg-darker: #020617;
            --text-light: #e2e8f0;
            --text-dim: #94a3b8;
            --success: #10b981;
            --error: #ef4444;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        @keyframes gradientShift {
            0% { background-position: 0% 50%; }
            50% { background-position: 100% 50%; }
            100% { background-position: 0% 50%; }
        }

        @keyframes float {
            0%, 100% { transform: translateY(0px) rotate(0deg); }
            33% { transform: translateY(-20px) rotate(2deg); }
            66% { transform: translateY(10px) rotate(-2deg); }
        }

        @keyframes slideInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.5; }
        }

        @keyframes glow {
            0%, 100% { 
                box-shadow: 0 0 20px rgba(0, 212, 255, 0.5),
                            0 0 40px rgba(0, 212, 255, 0.3),
                            0 0 60px rgba(0, 212, 255, 0.2);
            }
            50% { 
                box-shadow: 0 0 30px rgba(0, 212, 255, 0.8),
                            0 0 60px rgba(0, 212, 255, 0.5),
                            0 0 90px rgba(0, 212, 255, 0.3);
            }
        }

        @keyframes ripple {
            0% {
                transform: scale(0);
                opacity: 1;
            }
            100% {
                transform: scale(2);
                opacity: 0;
            }
        }

        body {
            font-family: 'Poppins', sans-serif;
            background: linear-gradient(-45deg, #020617, #0f172a, #1e293b, #334155);
            background-size: 400% 400%;
            animation: gradientShift 15s ease infinite;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
            position: relative;
            overflow: hidden;
        }

        /* Animated background particles */
        body::before {
            content: '';
            position: absolute;
            width: 500px;
            height: 500px;
            background: radial-gradient(circle, rgba(0, 212, 255, 0.15) 0%, transparent 70%);
            top: -150px;
            right: -150px;
            border-radius: 50%;
            animation: float 8s ease-in-out infinite;
            filter: blur(40px);
        }

        body::after {
            content: '';
            position: absolute;
            width: 400px;
            height: 400px;
            background: radial-gradient(circle, rgba(99, 102, 241, 0.15) 0%, transparent 70%);
            bottom: -100px;
            left: -100px;
            border-radius: 50%;
            animation: float 10s ease-in-out infinite reverse;
            filter: blur(40px);
        }

        /* Grid pattern overlay */
        .grid-overlay {
            position: absolute;
            inset: 0;
            background-image: 
                linear-gradient(rgba(0, 212, 255, 0.03) 1px, transparent 1px),
                linear-gradient(90deg, rgba(0, 212, 255, 0.03) 1px, transparent 1px);
            background-size: 50px 50px;
            pointer-events: none;
            opacity: 0.5;
        }

        .login-container {
            background: rgba(15, 23, 42, 0.7);
            backdrop-filter: blur(20px) saturate(180%);
            border: 1px solid rgba(0, 212, 255, 0.2);
            border-radius: 24px;
            box-shadow: 
                0 20px 60px rgba(0, 0, 0, 0.5),
                0 0 100px rgba(0, 212, 255, 0.1),
                inset 0 1px 0 rgba(255, 255, 255, 0.1);
            overflow: hidden;
            max-width: 450px;
            width: 100%;
            position: relative;
            z-index: 1;
            animation: slideInUp 0.8s cubic-bezier(0.16, 1, 0.3, 1);
        }

        /* Glowing border effect */
        .login-container::before {
            content: '';
            position: absolute;
            inset: -2px;
            background: linear-gradient(45deg, var(--primary), var(--secondary), var(--accent), var(--primary));
            background-size: 400% 400%;
            border-radius: 24px;
            z-index: -1;
            opacity: 0.5;
            animation: gradientShift 3s ease infinite;
            filter: blur(10px);
        }

        .login-header {
            padding: 50px 40px 40px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }

        .login-header::before {
            content: '';
            position: absolute;
            top: 0;
            left: 50%;
            transform: translateX(-50%);
            width: 200%;
            height: 100%;
            background: radial-gradient(circle, rgba(0, 212, 255, 0.1) 0%, transparent 70%);
            animation: pulse 3s ease-in-out infinite;
        }

        .logo-container {
            width: 100px;
            height: 100px;
            margin: 0 auto 30px;
            position: relative;
            animation: float 6s ease-in-out infinite;
        }

        .logo-ring {
            position: absolute;
            inset: 0;
            border-radius: 50%;
            border: 2px solid var(--primary);
            animation: glow 2s ease-in-out infinite;
        }

        .logo-ring::before,
        .logo-ring::after {
            content: '';
            position: absolute;
            inset: -10px;
            border-radius: 50%;
            border: 1px solid var(--primary);
            opacity: 0.3;
        }

        .logo-ring::after {
            inset: -20px;
            opacity: 0.15;
        }

        .logo-inner {
            position: relative;
            width: 100%;
            height: 100%;
            background: linear-gradient(135deg, var(--primary) 0%, var(--secondary) 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            box-shadow: 
                0 10px 40px rgba(0, 212, 255, 0.4),
                inset 0 -2px 10px rgba(0, 0, 0, 0.3);
        }

        .logo-inner svg {
            width: 50px;
            height: 50px;
            filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
        }

        .login-header h1 {
            font-family: 'Orbitron', sans-serif;
            font-size: 36px;
            font-weight: 900;
            color: var(--text-light);
            margin-bottom: 10px;
            position: relative;
            z-index: 1;
            letter-spacing: 2px;
            text-transform: uppercase;
            background: linear-gradient(135deg, var(--primary) 0%, #fff 50%, var(--primary) 100%);
            background-size: 200% 200%;
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
            animation: gradientShift 3s ease infinite;
        }

        .login-header p {
            color: var(--text-dim);
            font-size: 15px;
            font-weight: 300;
            position: relative;
            z-index: 1;
            letter-spacing: 1px;
        }

        .login-body {
            padding: 40px 40px 45px;
        }

        .form-group {
            margin-bottom: 25px;
            animation: slideInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1);
            animation-fill-mode: both;
        }

        .form-group:nth-child(2) { animation-delay: 0.1s; }
        .form-group:nth-child(3) { animation-delay: 0.2s; }
        .form-group:nth-child(4) { animation-delay: 0.3s; }

        .form-label {
            display: block;
            margin-bottom: 10px;
            color: var(--text-light);
            font-weight: 600;
            font-size: 13px;
            letter-spacing: 1px;
            text-transform: uppercase;
        }

        .input-wrapper {
            position: relative;
        }

        .input-icon {
            position: absolute;
            left: 18px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--text-dim);
            pointer-events: none;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            z-index: 2;
        }

        .form-control {
            width: 100%;
            padding: 16px 18px 16px 52px;
            border: 2px solid rgba(0, 212, 255, 0.2);
            border-radius: 12px;
            font-size: 15px;
            font-family: 'Poppins', sans-serif;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            background: rgba(15, 23, 42, 0.5);
            color: var(--text-light);
            position: relative;
        }

        .form-control::placeholder {
            color: var(--text-dim);
            opacity: 0.6;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary);
            background: rgba(15, 23, 42, 0.8);
            box-shadow: 
                0 0 0 4px rgba(0, 212, 255, 0.1),
                0 8px 20px rgba(0, 212, 255, 0.2);
            transform: translateY(-2px);
        }

        .form-control:focus + .input-icon {
            color: var(--primary);
            transform: translateY(-50%) scale(1.1);
        }

        .checkbox-wrapper {
            display: flex;
            align-items: center;
            gap: 12px;
            animation: slideInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1) 0.35s;
            animation-fill-mode: both;
            margin-top: 20px;
        }

        .checkbox-wrapper input[type="checkbox"] {
            width: 20px;
            height: 20px;
            border-radius: 6px;
            cursor: pointer;
            accent-color: var(--primary);
            position: relative;
        }

        .checkbox-wrapper label {
            color: var(--text-dim);
            font-size: 14px;
            cursor: pointer;
            user-select: none;
            transition: color 0.3s ease;
        }

        .checkbox-wrapper:hover label {
            color: var(--text-light);
        }

        .btn-login {
            width: 100%;
            padding: 18px;
            background: linear-gradient(135deg, var(--primary) 0%, var(--primary-dark) 100%);
            color: #000;
            border: none;
            border-radius: 12px;
            font-size: 16px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            margin-top: 15px;
            font-family: 'Poppins', sans-serif;
            letter-spacing: 1px;
            text-transform: uppercase;
            box-shadow: 
                0 10px 30px rgba(0, 212, 255, 0.3),
                inset 0 -2px 8px rgba(0, 0, 0, 0.2);
            position: relative;
            overflow: hidden;
            animation: slideInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1) 0.4s;
            animation-fill-mode: both;
        }

        .btn-login::before {
            content: '';
            position: absolute;
            top: 50%;
            left: 50%;
            width: 0;
            height: 0;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.3);
            transform: translate(-50%, -50%);
            transition: width 0.6s, height 0.6s;
        }

        .btn-login:hover {
            transform: translateY(-3px);
            box-shadow: 
                0 15px 40px rgba(0, 212, 255, 0.5),
                inset 0 -2px 8px rgba(0, 0, 0, 0.2);
        }

        .btn-login:hover::before {
            width: 300px;
            height: 300px;
        }

        .btn-login:active {
            transform: translateY(-1px);
        }

        .error-message {
            background: rgba(239, 68, 68, 0.1);
            border: 1px solid rgba(239, 68, 68, 0.3);
            color: #fca5a5;
            padding: 14px 18px;
            border-radius: 12px;
            margin-bottom: 25px;
            font-size: 14px;
            border-left: 4px solid var(--error);
            animation: slideInUp 0.3s ease-out;
            display: flex;
            align-items: center;
            gap: 12px;
            backdrop-filter: blur(10px);
        }

        .error-message::before {
            content: '⚠';
            font-size: 20px;
            animation: pulse 2s ease-in-out infinite;
        }

        /* Particle effect on hover */
        .particle {
            position: absolute;
            width: 4px;
            height: 4px;
            background: var(--primary);
            border-radius: 50%;
            pointer-events: none;
            animation: ripple 1s ease-out;
        }

        @media (max-width: 480px) {
            .login-container {
                border-radius: 20px;
            }
            
            .login-header {
                padding: 40px 30px 30px;
            }
            
            .login-header h1 {
                font-size: 28px;
            }
            
            .logo-container {
                width: 80px;
                height: 80px;
            }
            
            .login-body {
                padding: 35px 30px 40px;
            }
        }

        /* Style cho nút hiện mật khẩu */
        .toggle-password {
            position: absolute;
            right: 18px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: var(--text-dim);
            transition: all 0.3s ease;
            z-index: 3;
            display: flex;
            align-items: center;
        }

        .toggle-password:hover {
            color: var(--primary);
        }

        /* Hiệu ứng khi đang ở chế độ ẩn (tùy chọn: thêm gạch chéo qua mắt) */
        .toggle-password.slash .eye-icon {
            opacity: 0.7;
        }

        /* Loading state */
        .btn-login.loading {
            pointer-events: none;
            opacity: 0.7;
        }

        .btn-login.loading::after {
            content: '';
            position: absolute;
            width: 20px;
            height: 20px;
            top: 50%;
            left: 50%;
            margin-left: -10px;
            margin-top: -10px;
            border: 3px solid rgba(0, 0, 0, 0.3);
            border-top-color: #000;
            border-radius: 50%;
            animation: spin 0.6s linear infinite;
        }

        @keyframes spin {
            to { transform: rotate(360deg); }
        }
    </style>
</head>
<body>
    <div class="grid-overlay"></div>
    
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-header">
                <div class="logo-container">
                    <div class="logo-ring"></div>
                    <div class="logo-inner">
                        <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M12 2L2 7L12 12L22 7L12 2Z" fill="#0f172a" stroke="#0f172a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                            <path d="M2 17L12 22L22 17" stroke="#0f172a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                            <path d="M2 12L12 17L22 12" stroke="#0f172a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                    </div>
                </div>
                <h1>Unix System</h1>
                <p>Nhập thông tin để tiếp tục</p>
            </div>
            
            <div class="login-body">
                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="error-message">
                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                </asp:Panel>

                <div class="form-group">
                    <label class="form-label">Tên đăng nhập</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtusername" runat="server" CssClass="form-control" placeholder="Nhập username"></asp:TextBox>
                        <svg class="input-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M20 21V19C20 17.9391 19.5786 16.9217 18.8284 16.1716C18.0783 15.4214 17.0609 15 16 15H8C6.93913 15 5.92172 15.4214 5.17157 16.1716C4.42143 16.9217 4 17.9391 4 19V21" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                            <path d="M12 11C14.2091 11 16 9.20914 16 7C16 4.79086 14.2091 3 12 3C9.79086 3 8 4.79086 8 7C8 9.20914 9.79086 11 12 11Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label">Mật khẩu</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtpassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập password"></asp:TextBox>
                        <svg class="input-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                            <path d="M7 11V7C7 5.67392 7.52678 4.40215 8.46447 3.46447C9.40215 2.52678 10.6739 2 12 2C13.3261 2 14.5979 2.52678 15.5355 3.46447C16.4732 4.40215 17 5.67392 17 7V11" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                        <span id="togglePassword" class="toggle-password">
                            <svg class="eye-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                                <circle cx="12" cy="12" r="3"></circle>
                            </svg>
                        </span>
                    </div>
                </div>

                <div class="checkbox-wrapper">
                    <asp:CheckBox ID="chkRemember" runat="server" />
                    <label for="<%= chkRemember.ClientID %>">Ghi nhớ đăng nhập</label>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" CssClass="btn-login" OnClick="Loginclick" />
            </div>
        </div>
    </form>

    <script>
        // Add ripple effect on button click
        document.addEventListener('DOMContentLoaded', function() {
            const btn = document.querySelector('.btn-login');
            
            btn.addEventListener('click', function(e) {
                // Add loading state
                btn.classList.add('loading');
                
                // Create ripple effect
                const rect = btn.getBoundingClientRect();
                const ripple = document.createElement('span');
                ripple.className = 'particle';
                ripple.style.left = (e.clientX - rect.left) + 'px';
                ripple.style.top = (e.clientY - rect.top) + 'px';
                btn.appendChild(ripple);
                
                setTimeout(() => ripple.remove(), 1000);
            });
            
            // Input focus effects
            const inputs = document.querySelectorAll('.form-control');
            inputs.forEach(input => {
                input.addEventListener('focus', function() {
                    this.parentElement.style.transform = 'scale(1.02)';
                });
                
                input.addEventListener('blur', function() {
                    this.parentElement.style.transform = 'scale(1)';
                });
            });

            const togglePassword = document.querySelector('#togglePassword');
            const passwordInput = document.querySelector('#<%= txtpassword.ClientID %>');

            togglePassword.addEventListener('click', function () {
                // Kiểm tra loại input hiện tại
                const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
                passwordInput.setAttribute('type', type);

                // Thay đổi màu sắc hoặc icon để báo hiệu
                this.classList.toggle('active');
                if (type === 'text') {
                    this.style.color = 'var(--primary)';
                } else {
                    this.style.color = 'var(--text-dim)';
                }
            });
        });
    </script>
</body>
</html>
