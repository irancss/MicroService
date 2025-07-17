"use client";
import React, { useRef, useEffect } from "react";
import { Chart } from "chart.js";

const ChartComponent = ({ chartData, chartOptions }) => {
    const chartRef = useRef(null);
    useEffect(() => {
        const ctx = chartRef.current.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: chartData,
            options: chartOptions
        });
    }, [chartData, chartOptions]);

    return <canvas ref={chartRef} />;
};

export default ChartComponent;
