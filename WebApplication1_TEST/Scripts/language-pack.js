// ==========================================
// TỪ ĐIỂN ĐA NGÔN NGỮ (EN - ZH)
// ==========================================
const dictionary = {
    en: {
        // --- TỪ VỰNG FORM CPRDV02 ---
        cprdv02_machno: "MACHNO *",
        cprdv02_itnbr: "ITNBR",
        cprdv02_lmoldsize: "LMOLDSIZE",
        cprdv02_rmoldsize: "RMOLDSIZE",
        cprdv02_tireno: "TIRENO",
        cprdv02_ringno: "RINGNO",
        cprdv02_batchclamp: "BATCHCLAMP",
        cprdv02_specpci: "SPECPCI",
        cprdv02_version: "VERSION",
        cprdv02_stype: "STYPE",
        cprdv02_lbatchsize: "LBATCHSIZE",
        cprdv02_rbatchsize: "RBATCHSIZE",
        cprdv02_color: "COLOR",
        cprdv02_color1: "COLOR1",
        cprdv02_color2: "COLOR2",
        cprdv02_color3: "COLOR3",
        cprdv02_circolor: "CIRCOLOR",
        cprdv02_moldstyle: "MOLDSTYLE",
        cprdv02_tread: "TREAD",
        cprdv02_sidewall: "SIDEWALL",
        cprdv02_speed: "SPEED",
        cprdv02_structcod: "STRUCTCOD",
        cprdv02_dot: "DOT",
        cprdv02_maxload: "MAXLOAD",
        cprdv02_note1: "NOTE1",
        cprdv02_note2: "NOTE2",
        cprdv02_scansta: "SCANSTA",
        cprdv02_force: "FORCE",
        cprdv02_forcekn: "FORCEKN",
        cprdv02_heightpci: "HEIGHTPCI",
        cprdv02_timepci: "TIMEPCI",
        cprdv02_pressure: "PRESSURE",
        cprdv02_ltemp: "LTEMP",
        cprdv02_rtemp: "RTEMP",
        cprdv02_leptemp: "LEPTEMP",
        cprdv02_reptemp: "REPTEMP",
        cprdv02_lheight: "LHEIGHT",
        cprdv02_rheight: "RHEIGHT",
        cprdv02_ltype: "LTYPE",
        cprdv02_rtype: "RTYPE",
        cprdv02_lput: "LPUT",
        cprdv02_rput: "RPUT",
        cprdv02_rows: "Rows:",
        cprdv02_indat: "Indat:",
        cprdv02_user: "User:",

        // --- TỪ VỰNG FORM CPRDV01 ---
        cprdv01_subno: "subno",
        cprdv01_factory: "factory",
        cprdv01_itnbr: "itnbr",
        cprdv01_version: "VERSION",
        cprdv01_stype: "STYPE",
        cprdv01_machno: "machno",

        // machm 1-16
        cprdv01_machm1: "machm1", cprdv01_machm2: "machm2",
        cprdv01_machm3: "machm3", cprdv01_machm4: "machm4",
        cprdv01_machm5: "machm5", cprdv01_machm6: "machm6",
        cprdv01_machm7: "machm7", cprdv01_machm8: "machm8",
        cprdv01_machm9: "machm9", cprdv01_machm10: "machm10",
        cprdv01_machm11: "machm11", cprdv01_machm12: "machm12",
        cprdv01_machm13: "machm13", cprdv01_machm14: "machm14",
        cprdv01_machm15: "machm15", cprdv01_machm16: "machm16",

        // machs 1-16
        cprdv01_machs1: "machs1", cprdv01_machs2: "machs2",
        cprdv01_machs3: "machs3", cprdv01_machs4: "machs4",
        cprdv01_machs5: "machs5", cprdv01_machs6: "machs6",
        cprdv01_machs7: "machs7", cprdv01_machs8: "machs8",
        cprdv01_machs9: "machs9", cprdv01_machs10: "machs10",
        cprdv01_machs11: "machs11", cprdv01_machs12: "machs12",
        cprdv01_machs13: "machs13", cprdv01_machs14: "machs14",
        cprdv01_machs15: "machs15", cprdv01_machs16: "machs16",

        // check 1-16
        cprdv01_check1: "check1", cprdv01_check2: "check2",
        cprdv01_check3: "check3", cprdv01_check4: "check4",
        cprdv01_check5: "check5", cprdv01_check6: "check6",
        cprdv01_check7: "check7", cprdv01_check8: "check8",
        cprdv01_check9: "check9", cprdv01_check10: "check10",
        cprdv01_check11: "check11", cprdv01_check12: "check12",
        cprdv01_check13: "check13", cprdv01_check14: "check14",
        cprdv01_check15: "check15", cprdv01_check16: "check16",

        cprdv01_totalm: "totalm",
        cprdv01_totals: "total",
        cprdv01_ostoptime: "ostoptime",
        cprdv01_tstoptime: "tstoptime",
        cprdv01_oopentime: "oopentime",
        cprdv01_topentime: "topentime",
        cprdv01_opencheck: "opencheck",
        cprdv01_oplencheck: "oplencheck",
        cprdv01_cllencheck: "cllencheck",
        cprdv01_speedlen: "speedlen",
        cprdv01_opostopcheck: "opostopcheck",
        cprdv01_optstopcheck: "optstopcheck",
        cprdv01_clostopcheck: "clostopcheck",
        cprdv01_cltstopcheck: "cltstopcheck",
        cprdv01_wopcheck: "wopcheck",
        cprdv01_wclcheck: "wclcheck",
        cprdv01_opchecklen: "opchecklen",
        cprdv01_inocheck: "inocheck",
        cprdv01_ouocheck: "ouocheck",
        cprdv01_tocheck: "tocheck",
        cprdv01_lheight: "lheight",
        cprdv01_rheight: "rheight",
        cprdv01_ltype: "ltype",
        cprdv01_rtype: "rtype",
        cprdv01_pressure: "pressure",
        cprdv01_lput: "lput",
        cprdv01_rput: "rput",
        cprdv01_itdsc: "TEN HANG",
        cprdv01_indat_label: "NGAY NHAP",
        cprdv01_userno_label: "NGUOI NHAP",
        cprdv01_rows: "Rows"
    },
    zh: {
        // --- TỪ VỰNG FORM CPRDV02 ---
        cprdv02_machno: "機台號 *",
        cprdv02_itnbr: "成品代號",
        cprdv02_lmoldsize: "左模具規格",
        cprdv02_rmoldsize: "右模具規格",
        cprdv02_tireno: "二次生胎代號",
        cprdv02_ringno: "胎唇環型號",
        cprdv02_batchclamp: "氣囊夾環",
        cprdv02_specpci: "PCI輪輞規格",
        cprdv02_version: "版本",
        cprdv02_stype: "基準",
        cprdv02_lbatchsize: "左氣囊規格",
        cprdv02_rbatchsize: "右氣囊規格",
        cprdv02_color: "環保色線",
        cprdv02_color1: "色線1",
        cprdv02_color2: "色線2",
        cprdv02_color3: "色線3",
        cprdv02_circolor: "中心線",
        cprdv02_moldstyle: "分割機型式",
        cprdv02_tread: "TREAD",
        cprdv02_sidewall: "SIDEWALL",
        cprdv02_speed: "速度等級",
        cprdv02_structcod: "結構嗎",
        cprdv02_dot: "DOT",
        cprdv02_maxload: "MAXLOAD",
        cprdv02_note1: "NOTE1",
        cprdv02_note2: "NOTE2",
        cprdv02_scansta: "掃碼不成功禁止裝胎",
        cprdv02_force: "合模力(KN)",
        cprdv02_forcekn: "合模力上限報警值(KN)",
        cprdv02_heightpci: "PCI輪輞高度(mm)",
        cprdv02_timepci: "PCI輪輞充氣時間(Min)",
        cprdv02_pressure: "PCI輪輞壓力(Mpa)",
        cprdv02_ltemp: "左外溫溫度設定(度)",
        cprdv02_rtemp: "右外溫溫度設定(度)",
        cprdv02_leptemp: "左模套溫度設定(度)",
        cprdv02_reptemp: "右模套溫度設定(度)",
        cprdv02_lheight: "左上環拉伸高度(mm)",
        cprdv02_rheight: "右上環拉伸高度(mm)",
        cprdv02_ltype: "左上環整型高度(mm)",
        cprdv02_rtype: "右上環整型高度(mm)",
        cprdv02_lput: "左裝胎高度",
        cprdv02_rput: "右裝胎高度",
        cprdv02_rows: "行数:",
        cprdv02_indat: "創建時間:",
        cprdv02_user: "創建人:",

        // --- TỪ VỰNG FORM CPRDV01 ---
        cprdv01_subno: "公司別",
        cprdv01_factory: "生產廠",
        cprdv01_itnbr: "成品代號",
        cprdv01_version: "版本",
        cprdv01_stype: "基準",
        cprdv01_machno: "機型",

        // machm 1-16
        cprdv01_machm1: "步1設定時間(分)", cprdv01_machm2: "步2設定時間(分)",
        cprdv01_machm3: "步3設定時間(分)", cprdv01_machm4: "步4設定時間(分)",
        cprdv01_machm5: "步5設定時間(分)", cprdv01_machm6: "步6設定時間(分)",
        cprdv01_machm7: "步7設定時間(分)", cprdv01_machm8: "步8設定時間(分)",
        cprdv01_machm9: "步9設定時間(分)", cprdv01_machm10: "步10設定時間(分)",
        cprdv01_machm11: "步11設定時間(分)", cprdv01_machm12: "步12設定時間(分)",
        cprdv01_machm13: "步13設定時間(分)", cprdv01_machm14: "步14設定時間(分)",
        cprdv01_machm15: "步15設定時間(分)", cprdv01_machm16: "步16設定時間(分)",

        // machs 1-16
        cprdv01_machs1: "步1設定時間(秒)", cprdv01_machs2: "步2設定時間(秒)",
        cprdv01_machs3: "步3設定時間(秒)", cprdv01_machs4: "步4設定時間(秒)",
        cprdv01_machs5: "步5設定時間(秒)", cprdv01_machs6: "步6設定時間(秒)",
        cprdv01_machs7: "步7設定時間(秒)", cprdv01_machs8: "步8設定時間(秒)",
        cprdv01_machs9: "步9設定時間(秒)", cprdv01_machs10: "步10設定時間(秒)",
        cprdv01_machs11: "步11設定時間(秒)", cprdv01_machs12: "步12設定時間(秒)",
        cprdv01_machs13: "步13設定時間(秒)", cprdv01_machs14: "步14設定時間(秒)",
        cprdv01_machs15: "步15設定時間(秒)", cprdv01_machs16: "步16設定時間(秒)",

        // check 1-16
        cprdv01_check1: "步1閥門控制字", cprdv01_check2: "步2閥門控制字",
        cprdv01_check3: "步3閥門控制字", cprdv01_check4: "步4閥門控制字",
        cprdv01_check5: "步5閥門控制字", cprdv01_check6: "步6閥門控制字",
        cprdv01_check7: "步7閥門控制字", cprdv01_check8: "步8閥門控制字",
        cprdv01_check9: "步9閥門控制字", cprdv01_check10: "步10閥門控制字",
        cprdv01_check11: "步11閥門控制字", cprdv01_check12: "步12閥門控制字",
        cprdv01_check13: "步13閥門控制字", cprdv01_check14: "步14閥門控制字",
        cprdv01_check15: "步15閥門控制字", cprdv01_check16: "步16閥門控制字",

        cprdv01_totalm: "總時間(分)",
        cprdv01_totals: "總時間(秒)",
        cprdv01_ostoptime: "一次合模暫停時間",
        cprdv01_tstoptime: "二次合模暫停時間",
        cprdv01_oopentime: "一次開模暫停時間",
        cprdv01_topentime: "二次開模暫停時間",
        cprdv01_opencheck: "小排設定",
        cprdv01_oplencheck: "開模高度限制",
        cprdv01_cllencheck: "閉模高度限制",
        cprdv01_speedlen: "變速合模高度",
        cprdv01_opostopcheck: "合模一次暫停位置",
        cprdv01_optstopcheck: "合模二次暫停位置",
        cprdv01_clostopcheck: "開模一次暫停位置",
        cprdv01_cltstopcheck: "開模二次暫停位置",
        cprdv01_wopcheck: "活絡模開檢查位置",
        cprdv01_wclcheck: "活絡模閉檢查位置",
        cprdv01_opchecklen: "開模取胎高度限制",
        cprdv01_inocheck: "入模位置",
        cprdv01_ouocheck: "擺出位置",
        cprdv01_tocheck: "定型位置",
        cprdv01_lheight: "左上環拉伸高度(mm)",
        cprdv01_rheight: "右上環拉伸高度(mm)",
        cprdv01_ltype: "左上環整型高度(mm)",
        cprdv01_rtype: "右上環整型高度(mm)",
        cprdv01_pressure: "PCI輪輞壓力(Mpa)",
        cprdv01_lput: "左裝胎高度",
        cprdv01_rput: "右裝胎高度",
        cprdv01_itdsc: "TEN HANG",
        cprdv01_indat_label: "NGAY NHAP",
        cprdv01_userno_label: "NGUOI NHAP",
        cprdv01_rows: "Rows"
    }
};

function changeLanguage(lang) {
    document.querySelectorAll('.lang-btn').forEach(btn => btn.classList.remove('active'));

    const activeBtn = document.getElementById('btn-' + lang);
    if (activeBtn) {
        activeBtn.classList.add('active');
    }

    document.querySelectorAll('[data-i18n]').forEach(element => {
        const key = element.getAttribute('data-i18n');
        if (dictionary[lang] && dictionary[lang][key]) {
            element.innerHTML = dictionary[lang][key];
        }
    });

    localStorage.setItem('preferredLang', lang);
}

window.addEventListener('DOMContentLoaded', function () {
    const savedLang = localStorage.getItem('preferredLang') || 'en';
    changeLanguage(savedLang);
});