// A class for getting the day of the week that includes various methods, for example, from the current date to the past 30 days, the past three months, the past six months, the past one year.

class DateRangeHelper {
	static getDayOfWeek(date) {
		return date.toLocaleString('en-US', { weekday: 'long' });
	}

	static getPastDays(days) {
		const result = [];
		const today = new Date();
		for (let i = 0; i < days; i++) {
			const d = new Date(today);
			d.setDate(today.getDate() - i);
			result.push({
				date: d,
				dayOfWeek: this.getDayOfWeek(d)
			});
		}
		return result;
	}

	static getPastMonths(months) {
		const result = [];
		const today = new Date();
		for (let i = 0; i < months; i++) {
			const d = new Date(today);
			d.setMonth(today.getMonth() - i);
			result.push({
				date: d,
				dayOfWeek: this.getDayOfWeek(d)
			});
		}
		return result;
	}

	static getPastYears(years) {
		const result = [];
		const today = new Date();
		for (let i = 0; i < years; i++) {
			const d = new Date(today);
			d.setFullYear(today.getFullYear() - i);
			result.push({
				date: d,
				dayOfWeek: this.getDayOfWeek(d)
			});
		}
		return result;
	}
}

// Example usage:
// DateRangeHelper.getPastDays(30);
// DateRangeHelper.getPastMonths(3);
// DateRangeHelper.getPastMonths(6);
// DateRangeHelper.getPastYears(1);