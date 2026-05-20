// Analytics Dashboard JavaScript
(function () {
    'use strict';

    // Initialize all dashboard components
    function initAnalyticsDashboard() {
        initScoreChart();
        initInsightCards();
        initTableSorting();
    }

    // Score Distribution Chart
    function initScoreChart() {
        const chartElement = document.getElementById('scoreChart');
        if (!chartElement) return;

        const distribution = window.scoreDistribution || [];

        new Chart(chartElement.getContext('2d'), {
            type: 'bar',
            data: {
                labels: distribution.map(d => d.range),
                datasets: [{
                    label: 'Number of Students',
                    data: distribution.map(d => d.count),
                    backgroundColor: 'rgba(59, 130, 246, 0.7)',
                    borderColor: 'rgba(59, 130, 246, 1)',
                    borderWidth: 1,
                    borderRadius: 6,
                    barPercentage: 0.7,
                    categoryPercentage: 0.8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        backgroundColor: '#1f2937',
                        titleColor: '#f3f4f6',
                        bodyColor: '#d1d5db',
                        padding: 10,
                        cornerRadius: 8
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: '#e5e7eb', drawBorder: false },
                        title: { display: true, text: 'Students', color: '#6b7280', font: { size: 11, weight: '500' } },
                        ticks: { stepSize: 1, color: '#6b7280' }
                    },
                    x: {
                        grid: { display: false },
                        title: { display: true, text: 'Score Range (%)', color: '#6b7280', font: { size: 11, weight: '500' } },
                        ticks: { color: '#6b7280' }
                    }
                }
            }
        });
    }

    // Insight Cards Click Handler
    function initInsightCards() {
        document.querySelectorAll('.insight-card').forEach(card => {
            card.addEventListener('click', () => {
                const targetId = card.getAttribute('data-target');
                if (targetId) {
                    document.getElementById(targetId)?.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });
    }

    // Table Sorting (optional enhancement)
    function initTableSorting() {
        const tables = document.querySelectorAll('.data-table.sortable');
        // Add sorting functionality if needed
    }

    // Export initialization
    document.addEventListener('DOMContentLoaded', initAnalyticsDashboard);
})();