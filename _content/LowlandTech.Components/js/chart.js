// Chart.js interop for LowlandTech.Components.Chart
// Wraps Chart.js library for Blazor integration

const chartInstances = new Map();

/**
 * Initialize a new chart
 * @param {string} canvasId - The canvas element ID
 * @param {string} type - The chart type (bar, line, pie, doughnut, area)
 * @param {object} data - The chart data
 * @param {object} options - The chart options
 */
export function initChart(canvasId, type, data, options) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error(`Canvas element with id '${canvasId}' not found`);
        return;
    }

    // Destroy existing chart if present
    if (chartInstances.has(canvasId)) {
        const existing = chartInstances.get(canvasId);
        existing.destroy();
        chartInstances.delete(canvasId);
    }

    // Get Chart.js from global scope (must be loaded via script tag)
    const Chart = window.Chart;
    if (!Chart) {
        console.error('Chart.js library not loaded. Please include Chart.js script in your HTML.');
        return;
    }

    // Clone options to avoid mutations
    const chartOptions = JSON.parse(JSON.stringify(options || {}));

    // Set default font family
    Chart.defaults.font.family = 'system-ui, -apple-system, sans-serif';
    Chart.defaults.color = 'hsl(var(--foreground))';

    // Configure tooltip defaults for shadcn styling
    if (!chartOptions.plugins) chartOptions.plugins = {};
    if (!chartOptions.plugins.tooltip) chartOptions.plugins.tooltip = {};
    
    chartOptions.plugins.tooltip.backgroundColor = 'hsl(var(--popover))';
    chartOptions.plugins.tooltip.titleColor = 'hsl(var(--popover-foreground))';
    chartOptions.plugins.tooltip.bodyColor = 'hsl(var(--popover-foreground))';
    chartOptions.plugins.tooltip.borderColor = 'hsl(var(--border))';
    chartOptions.plugins.tooltip.borderWidth = 1;
    chartOptions.plugins.tooltip.padding = 12;
    chartOptions.plugins.tooltip.cornerRadius = 6;
    chartOptions.plugins.tooltip.displayColors = true;
    chartOptions.plugins.tooltip.usePointStyle = true;

    // Configure legend defaults
    if (!chartOptions.plugins.legend) chartOptions.plugins.legend = {};
    chartOptions.plugins.legend.labels = {
        usePointStyle: true,
        padding: 16,
        font: {
            size: 12
        }
    };

    // Configure grid styling
    if (chartOptions.scales) {
        ['x', 'y'].forEach(axis => {
            if (chartOptions.scales[axis]) {
                if (!chartOptions.scales[axis].grid) chartOptions.scales[axis].grid = {};
                chartOptions.scales[axis].grid.color = 'hsl(var(--border) / 0.3)';
                chartOptions.scales[axis].ticks = {
                    color: 'hsl(var(--muted-foreground))',
                    font: { size: 11 }
                };
                if (!chartOptions.scales[axis].border) chartOptions.scales[axis].border = {};
                chartOptions.scales[axis].border.display = false;
            }
        });
    }

    // Handle area chart type (line with fill)
    let chartType = type;
    if (type === 'area') {
        chartType = 'line';
    }

    // Clone data to avoid mutations
    const chartData = JSON.parse(JSON.stringify(data));

    // Apply fill for area charts
    if (type === 'area' && chartData.datasets) {
        chartData.datasets.forEach(dataset => {
            dataset.fill = true;
        });
    }

    // Create the chart
    const chart = new Chart(canvas, {
        type: chartType,
        data: chartData,
        options: chartOptions
    });

    chartInstances.set(canvasId, chart);
    return chart;
}

/**
 * Update an existing chart's data
 * @param {string} canvasId - The canvas element ID
 * @param {object} data - The new chart data
 */
export function updateChart(canvasId, data) {
    const chart = chartInstances.get(canvasId);
    if (!chart) {
        console.warn(`Chart with id '${canvasId}' not found`);
        return;
    }

    chart.data = JSON.parse(JSON.stringify(data));
    chart.update('active');
}

/**
 * Update chart options
 * @param {string} canvasId - The canvas element ID
 * @param {object} options - The new chart options
 */
export function updateChartOptions(canvasId, options) {
    const chart = chartInstances.get(canvasId);
    if (!chart) {
        console.warn(`Chart with id '${canvasId}' not found`);
        return;
    }

    chart.options = JSON.parse(JSON.stringify(options));
    chart.update();
}

/**
 * Destroy a chart instance
 * @param {string} canvasId - The canvas element ID
 */
export function destroyChart(canvasId) {
    const chart = chartInstances.get(canvasId);
    if (chart) {
        chart.destroy();
        chartInstances.delete(canvasId);
    }
}

/**
 * Resize a chart
 * @param {string} canvasId - The canvas element ID
 */
export function resizeChart(canvasId) {
    const chart = chartInstances.get(canvasId);
    if (chart) {
        chart.resize();
    }
}

/**
 * Get chart instance (for debugging)
 * @param {string} canvasId - The canvas element ID
 */
export function getChart(canvasId) {
    return chartInstances.get(canvasId);
}
