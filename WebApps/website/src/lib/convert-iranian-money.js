// تبدیل رشته پول ایرانی به عدد
export function convertIranianMoneyToNumber(moneyString) {
    const cleanedString = moneyString.replace(/[^0-9]/g, "");
    return parseInt(cleanedString, 10);
}

// تبدیل ریال به تومان
export function convertNumberRialToToman(rial) {
    return Math.floor(rial / 10);
}

// تبدیل تومان به ریال
export function convertNumberTomanToRial(toman) {
    return toman * 10;
}

// تبدیل عدد ریال به حروف تومان
export function convertNumberRialToLettersToman(rial) {
    const toman = convertNumberRialToToman(rial);
    return convertNumberToLetters(toman);
}

// تبدیل عدد به حروف فارسی (تا میلیارد)
export function convertNumberToLetters(number) {
    const units = ["", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه"];
    const teens = [
        "ده", "یازده", "دوازده", "سیزده", "چهارده",
        "پانزده", "شانزده", "هفده", "هجده", "نوزده"
    ];
    const tens = [
        "", "", "بیست", "سی", "چهل", "پنجاه",
        "شصت", "هفتاد", "هشتاد", "نود"
    ];
    const hundreds = [
        "", "صد", "دویست", "سیصد", "چهارصد",
        "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد"
    ];

    if (number === 0) return "صفر";
    if (number < 0) return "منفی " + convertNumberToLetters(-number);

    let words = [];

    if (number >= 1000000000) {
        let billions = Math.floor(number / 1000000000);
        words.push(convertNumberToLetters(billions) + " میلیارد");
        number %= 1000000000;
    }
    if (number >= 1000000) {
        let millions = Math.floor(number / 1000000);
        words.push(convertNumberToLetters(millions) + " میلیون");
        number %= 1000000;
    }
    if (number >= 1000) {
        let thousands = Math.floor(number / 1000);
        words.push(convertNumberToLetters(thousands) + " هزار");
        number %= 1000;
    }
    if (number >= 100) {
        words.push(hundreds[Math.floor(number / 100)]);
        number %= 100;
    }
    if (number >= 20) {
        words.push(tens[Math.floor(number / 10)]);
        number %= 10;
    }
    if (number >= 10) {
        words.push(teens[number - 10]);
        number = 0;
    }
    if (number > 0) {
        words.push(units[number]);
    }

    // حذف فاصله‌های اضافی و اتصال با " و "
    return words.filter(Boolean).join(" و ");
}

// تبدیل حروف فارسی به عدد (محدود و ساده)
export function convertLettersToNumber(letters) {
    const map = {
        "یک": 1, "دو": 2, "سه": 3, "چهار": 4, "پنج": 5,
        "شش": 6, "هفت": 7, "هشت": 8, "نه": 9, "ده": 10,
        "یازده": 11, "دوازده": 12, "سیزده": 13, "چهارده": 14,
        "پانزده": 15, "شانزده": 16, "هفده": 17, "هجده": 18, "نوزده": 19,
        "بیست": 20, "سی": 30, "چهل": 40, "پنجاه": 50, "شصت": 60,
        "هفتاد": 70, "هشتاد": 80, "نود": 90, "صد": 100, "دویست": 200,
        "سیصد": 300, "چهارصد": 400, "پانصد": 500, "ششصد": 600,
        "هفتصد": 700, "هشتصد": 800, "نهصد": 900, "هزار": 1000
    };

    let total = 0;
    let parts = letters.split(" و ");
    for (let part of parts) {
        if (part.includes("هزار")) {
            let num = part.replace(" هزار", "");
            total += (map[num] || 1) * 1000;
        } else {
            total += map[part.trim()] || 0;
        }
    }
    return total;
}

// تبدیل عدد ریال به حرف تومان
export function convertNumberRialToLetters(rial) {
    const toman = convertNumberRialToToman(rial);
    return convertNumberToLetters(toman);
}
