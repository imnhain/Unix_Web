<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Unix_Web.Shared.Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard - Kendatire System
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="server">
    Dashboard
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* CSS cho Canvas Hiệu ứng Lễ hội */
        #holidayCanvas {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none; /* Không cản trở người dùng click chuột */
            z-index: 9999;
            display: none;
        }

        /* Welcome Banner */
        .welcome-banner {
            background: linear-gradient(135deg, rgba(0, 212, 255, 0.1) 0%, rgba(99, 102, 241, 0.1) 100%);
            border: 2px solid rgba(0, 212, 255, 0.3);
            border-radius: 20px;
            padding: 40px;
            margin-bottom: 30px;
            display: flex;
            align-items: center;
            gap: 30px;
            backdrop-filter: blur(20px);
            box-shadow: 0 10px 40px rgba(0, 212, 255, 0.2);
            position: relative;
            overflow: hidden;
        }

        .welcome-banner::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -10%;
            width: 400px;
            height: 400px;
            background: radial-gradient(circle, rgba(0, 212, 255, 0.2) 0%, transparent 70%);
            border-radius: 50%;
            animation: float 8s ease-in-out infinite;
        }

        @keyframes float {
            0%, 100% { transform: translateY(0px) rotate(0deg); }
            50% { transform: translateY(-20px) rotate(5deg); }
        }

        .kendatire-logo {
            width: 120px;
            height: 120px;
            background: white;
            border-radius: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
            flex-shrink: 0;
            position: relative;
            z-index: 1;
            padding: 15px;
        }

        .kendatire-logo img {
            width: 100%;
            height: 100%;
            object-fit: contain;
        }

        .logo-placeholder {
            width: 100%;
            height: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Orbitron', sans-serif;
            font-size: 24px;
            font-weight: 900;
            background: linear-gradient(135deg, #00d4ff 0%, #6366f1 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
            text-align: center;
            line-height: 1.2;
        }

        .welcome-content {
            flex: 1;
            position: relative;
            z-index: 1;
        }

        .welcome-title {
            font-size: 32px;
            font-weight: 700;
            color: var(--text-light, #fff);
            margin-bottom: 10px;
        }

        .welcome-title span {
            background: linear-gradient(135deg, var(--primary, #00d4ff) 0%, var(--secondary, #6366f1) 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }

        .welcome-subtitle {
            font-size: 16px;
            color: var(--text-dim, #ccc);
            margin-bottom: 20px;
        }

        .welcome-info {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
            align-items: flex-start;
        }

        .info-badge {
            display: flex;
            align-items: center;
            gap: 8px;
            padding: 8px 16px;
            background: rgba(0, 212, 255, 0.1);
            border: 1px solid rgba(0, 212, 255, 0.3);
            border-radius: 10px;
            font-size: 14px;
            color: #fff;
        }

        .info-badge svg {
            width: 18px;
            height: 18px;
            stroke: var(--primary, #00d4ff);
        }

        /* Tinh chỉnh Badge Đếm ngược để chứa 2 dòng */
        .countdown-badge {
            background: rgba(16, 185, 129, 0.15);
            border-color: rgba(16, 185, 129, 0.4);
            box-shadow: 0 0 15px rgba(16, 185, 129, 0.2);
            align-items: flex-start; /* Canh trên cho icon */
        }
        
        .countdown-badge svg {
            stroke: #10b981;
            margin-top: 2px; /* Chỉnh icon ngang hàng với dòng 1 */
        }

        .countdown-text-container {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        #holidayName {
            color: #10b981;
            font-weight: 600;
            font-size: 15px;
        }

        #countdown {
            color: rgba(255, 255, 255, 0.9);
            font-size: 13px;
            font-family: monospace;
            letter-spacing: 0.5px;
        }

        @media (max-width: 768px) {
            .welcome-banner {
                flex-direction: column;
                text-align: center;
                padding: 30px 20px;
            }

            .kendatire-logo {
                width: 100px;
                height: 100px;
            }

            .welcome-title {
                font-size: 24px;
            }

            .welcome-info {
                justify-content: center;
            }
        }
    </style>

    <canvas id="holidayCanvas"></canvas>

    <div class="welcome-banner">
        <div class="kendatire-logo">
            <asp:Image ID="imgKendatireLogo" runat="server" ImageUrl="~/Content/images/kendatire-logo.png" AlternateText="Kendatire" 
                       onerror="this.style.display='none'; this.nextElementSibling.style.display='flex';" />
            <div class="logo-placeholder" style="display: none;">
                KENDA<br/>TIRE
            </div>
        </div>
        <div class="welcome-content">
            <h1 class="welcome-title">
                Chào mừng trở lại, <span><asp:Label ID="lblWelcomeUser" runat="server">Admin</asp:Label></span>!
            </h1>
            <p class="welcome-subtitle">
                Hệ thống quản lý Kendatire - Dashboard tổng quan
            </p>
            <div class="welcome-info">
                
                <div class="info-badge">
                    <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
                        <path d="M12 6V12L16 14" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
                    </svg>
                    <span id="realtimeClock">
                        <asp:Label ID="lblCurrentTime" runat="server">Đang tải giờ...</asp:Label>
                    </span>
                </div>
                
                <div class="info-badge countdown-badge">
                    <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M8 2V5M16 2V5M3.5 9.09H20.5M21 8.5V17C21 20 19.5 22 16 22H8C4.5 22 3 20 3 17V8.5C3 5.5 4.5 3.5 8 3.5H16C19.5 3.5 21 5.5 21 8.5Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                        <path d="M11.9955 13.7H12.0045M8.2943 13.7H8.30329M8.2943 16.7H8.30329" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                    <div class="countdown-text-container">
                        <span id="holidayName">Đang quét dữ liệu...</span>
                        <span id="countdown"></span>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        console.log('Kendatire Dashboard loaded successfully');

        // ==========================================
        // 1. CẬP NHẬT ĐỒNG HỒ REAL-TIME
        // ==========================================
        function updateRealTimeClock() {
            const now = new Date();
            const daysOfWeek = ["Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy"];

            const dayName = daysOfWeek[now.getDay()];
            const dd = String(now.getDate()).padStart(2, '0');
            const mm = String(now.getMonth() + 1).padStart(2, '0');
            const yyyy = now.getFullYear();

            const hh = String(now.getHours()).padStart(2, '0');
            const min = String(now.getMinutes()).padStart(2, '0');

            const timeString = `${dayName}, ${dd}/${mm}/${yyyy} ${hh}:${min}`;
            document.getElementById('realtimeClock').innerText = timeString;
        }

        updateRealTimeClock();
        setInterval(updateRealTimeClock, 1000);

        // ==========================================
        // 2. LOGIC ĐẾM NGƯỢC NGÀY LỄ
        // ==========================================
        function getHolidays() {
            const currentYear = new Date().getFullYear();
            let holidaysList = [];

            const lunarDates = {
                tetNguyenDan: {
                    2026: '02-17', 2027: '02-06', 2028: '01-26', 2029: '02-13', 2030: '02-03',
                    2031: '01-23', 2032: '02-11', 2033: '01-31', 2034: '02-19', 2035: '02-08',
                    2036: '01-28', 2037: '02-15', 2038: '02-04', 2039: '01-24', 2040: '02-12',
                    2041: '02-01', 2042: '01-22', 2043: '02-10', 2044: '01-30', 2045: '02-17'
                },
                gioTo: {
                    2026: '04-26', 2027: '04-16', 2028: '04-04', 2029: '04-23', 2030: '04-11',
                    2031: '04-30', 2032: '04-19', 2033: '04-09', 2034: '04-28', 2035: '04-17',
                    2036: '04-06', 2037: '04-24', 2038: '04-14', 2039: '04-03', 2040: '04-21',
                    2041: '04-10', 2042: '04-29', 2043: '04-19', 2044: '04-07', 2045: '04-26'
                },
                trungThu: {
                    2026: '09-25', 2027: '09-15', 2028: '10-03', 2029: '09-22', 2030: '09-12',
                    2031: '10-01', 2032: '09-19', 2033: '09-08', 2034: '09-27', 2035: '09-16',
                    2036: '10-04', 2037: '09-24', 2038: '09-13', 2039: '10-02', 2040: '09-21',
                    2041: '09-10', 2042: '09-28', 2043: '09-17', 2044: '10-05', 2045: '09-25'
                }
            };

            for (let year = currentYear; year <= currentYear + 1; year++) {
                holidaysList.push({ id: 'newyear', name: `✨ Tết Dương lịch`, date: new Date(`${year}-01-01T00:00:00`).getTime() });
                holidaysList.push({ id: '30thang4', name: `Giải phóng Miền Nam 30/4`, date: new Date(`${year}-04-30T00:00:00`).getTime() });
                holidaysList.push({ id: '1thang5', name: `Quốc tế Lao động 1/5`, date: new Date(`${year}-05-01T00:00:00`).getTime() });
                holidaysList.push({ id: '2thang9', name: `Quốc khánh 2/9`, date: new Date(`${year}-09-02T00:00:00`).getTime() });
                holidaysList.push({ id: 'noel', name: `🎄 Noel 25/12`, date: new Date(`${year}-12-25T00:00:00`).getTime() });

                if (lunarDates.tetNguyenDan[year]) holidaysList.push({ id: 'tet', name: `🧧 Tết Nguyên Đán`, date: new Date(`${year}-${lunarDates.tetNguyenDan[year]}T00:00:00`).getTime() });
                if (lunarDates.gioTo[year]) holidaysList.push({ id: 'gioto', name: `Giỗ tổ Hùng Vương`, date: new Date(`${year}-${lunarDates.gioTo[year]}T00:00:00`).getTime() });
                if (lunarDates.trungThu[year]) holidaysList.push({ id: 'trungthu', name: `🏮 Tết Trung Thu`, date: new Date(`${year}-${lunarDates.trungThu[year]}T00:00:00`).getTime() });
            }

            holidaysList.sort((a, b) => a.date - b.date);
            return holidaysList;
        }

        const allHolidays = getHolidays();
        let effectTriggered = false; // Đảm bảo hiệu ứng chỉ được kích hoạt 1 lần

        const countdownTimer = setInterval(function () {
            const now = new Date();
            const nowTime = now.getTime();

            let nextHoliday = null;
            for (let i = 0; i < allHolidays.length; i++) {
                if (allHolidays[i].date + 86400000 > nowTime) {
                    nextHoliday = allHolidays[i];
                    break;
                }
            }

            if (!nextHoliday) return;

            // Nếu HÔM NAY là ngày lễ
            if (nowTime >= nextHoliday.date && nowTime < nextHoliday.date + 86400000) {
                document.getElementById("holidayName").innerHTML = `🎉 Hôm nay là ${nextHoliday.name}!`;
                document.getElementById("countdown").innerHTML = "Chúc mọi người đi làm ngày lễ vui vẻ =))))))))))";

                // Kích hoạt hiệu ứng canvas tương ứng với ngày lễ (Chỉ gọi 1 lần)
                if (!effectTriggered) {
                    startHolidayEffect(nextHoliday.id);
                    effectTriggered = true;
                }
                return;
            }

            // Xử lý đếm ngược (nếu chưa tới)
            let countMondays = 0;
            let tempDate = new Date(nowTime);
            tempDate.setHours(0, 0, 0, 0);

            let targetDate = new Date(nextHoliday.date);
            targetDate.setHours(0, 0, 0, 0);

            tempDate.setDate(tempDate.getDate() + 1);
            while (tempDate <= targetDate) {
                if (tempDate.getDay() === 1) countMondays++;
                tempDate.setDate(tempDate.getDate() + 1);
            }

            if (countMondays > 0) {
                document.getElementById("holidayName").innerHTML = `Chỉ còn ${countMondays} cái Thứ 2 nữa là tới ${nextHoliday.name}!`;
            } else {
                document.getElementById("holidayName").innerHTML = `Sắp tới: ${nextHoliday.name}`;
            }

            const distance = nextHoliday.date - nowTime;
            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            document.getElementById("countdown").innerHTML =
                `⏱️ Countdown: ${days} ngày ${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

        }, 1000);


        // ==========================================
        // 3. HỆ THỐNG CANVAS RƠI VẬT THỂ
        // ==========================================
        function startHolidayEffect(holidayId) {
            const canvas = document.getElementById('holidayCanvas');
            canvas.style.display = 'block';
            const ctx = canvas.getContext('2d');

            // Fix kích thước canvas mượt mà trên mọi màn hình
            let width = window.innerWidth;
            let height = window.innerHeight;
            canvas.width = width;
            canvas.height = height;

            window.addEventListener('resize', function () {
                width = window.innerWidth;
                height = window.innerHeight;
                canvas.width = width;
                canvas.height = height;
            });

            // Khai báo mảng icon (Emoji) cho từng loại lễ
            let icons = [];
            if (holidayId === '30thang4' || holidayId === '2thang9' || holidayId === '1thang5') {
                icons = ['⭐'];
            } else if (holidayId === 'trungthu') {
                icons = ['🏮', '🥮', '🌕'];
            } else if (holidayId === 'tet') {
                icons = ['🌸', '🧧', '🏮'];
            } else if (holidayId === 'noel') {
                icons = ['❄️', '🎄', '🎁', '⛄'];
            } else if (holidayId === 'newyear') {
                icons = ['🎉', '🎆', '🥂', '🎊'];
            } else {
                icons = ['✨', '🌟'];
            }

            // Tạo hạt (Particle)
            const particles = [];
            const maxParticles = 40; // Số lượng vật thể rơi (để thấp tránh lag)

            for (let i = 0; i < maxParticles; i++) {
                particles.push({
                    x: Math.random() * width,
                    y: Math.random() * height - height, // Bắt đầu rơi từ trên màn hình
                    r: Math.random() * 20 + 15,         // Kích thước (font-size)
                    speedY: Math.random() * 2 + 1,      // Tốc độ rơi thẳng
                    speedX: Math.random() * 1 - 0.5,    // Tốc độ gió thổi ngang
                    rotation: Math.random() * 360,      // Góc xoay ban đầu
                    rotationSpeed: Math.random() * 2 - 1, // Tốc độ xoay
                    icon: icons[Math.floor(Math.random() * icons.length)] // Icon ngẫu nhiên
                });
            }

            const startTime = Date.now();
            const duration = 60000; // 60,000 ms = 1 phút (Bạn có thể đổi thành 10000 để test 10 giây)
            let isFinishing = false; // Cờ báo hiệu bắt đầu dọn dẹp

            // Vòng lặp Animation
            function draw() {
                ctx.clearRect(0, 0, width, height);

                // Nếu đã qua 1 phút, bật cờ Finishing
                if (Date.now() - startTime > duration) {
                    isFinishing = true;
                }

                let particlesVisible = false; // Biến kiểm tra xem còn hạt nào trên màn hình không

                for (let i = 0; i < particles.length; i++) {
                    let p = particles[i];

                    // Vẽ vật thể
                    ctx.save();
                    ctx.translate(p.x, p.y);
                    ctx.rotate(p.rotation * Math.PI / 180);
                    ctx.font = p.r + 'px Arial';
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText(p.icon, 0, 0);
                    ctx.restore();

                    // Cập nhật vị trí cho khung hình tiếp theo
                    p.y += p.speedY;
                    p.x += p.speedX;
                    p.rotation += p.rotationSpeed;

                    // Xử lý khi hạt rơi xuống quá mép dưới màn hình
                    if (p.y > height + 50) {
                        // Nếu chưa hết 1 phút (chưa Finishing), cho hạt quay lại trên đỉnh
                        if (!isFinishing) {
                            p.y = -50;
                            p.x = Math.random() * width;
                            particlesVisible = true;
                        }
                    } else {
                        // Hạt vẫn đang bay giữa màn hình
                        particlesVisible = true;
                    }
                }

                // CHỐT CHẶN: Nếu còn hạt trên màn hình thì vẽ tiếp, nếu không thì nghỉ luôn
                if (particlesVisible) {
                    requestAnimationFrame(draw);
                } else {
                    // Dọn dẹp triệt để
                    canvas.style.display = 'none';
                    ctx.clearRect(0, 0, width, height);
                }
            }

            draw();
        }
    </script>
</asp:Content>