/*
* Refactored benchmark chart renderer
* ----------------------------------
*
* What this file does
* - Extracts series from window.BENCHMARK_DATA
* - Builds a responsive card (title + chart container) per benchmark -> Expect Tailwind
* - Renders a line chart via ApexCharts with optional expected-value annotations
*
* Expected external globals
* - window.BENCHMARK_DATA: { entries: { Benchmark: Array<{ date: string, benches: Array<{ name: string, value: number }> }> } }
* - ApexCharts: provided by <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
* - TailwindCSS : provided by <script src="https://cdn.tailwindcss.com"></script>
* - Flowbite : provided by <script src="https://cdn.jsdelivr.net/npm/flowbite@3.1.2/dist/flowbite.min.js"></script> & <link href="https://cdn.jsdelivr.net/npm/flowbite@3.1.2/dist/flowbite.min.css" rel="stylesheet" />
*/


/**
* Optional benchmark ranges used for annotations and y‑axis padding.
* You can tailor these per benchmark method without touching the code.
*/

BENCHMARK_RANGES = {
    SpeedBenchmarks: {
        Default_MeshToString:
        {
            mean_ms_min: 100.0,
            mean_ms_max: 250.0
        },
        Occluder: {
            mean_ms_max: 50.0,
            error_ratio_max: 0.10,
            alloc_bytes_max: 8000000
        },
        Optimize: {
            mean_ms_max: 50.0,
            error_ratio_max: 0.10,
            alloc_bytes_max: 12000000
        },
        Occluder_MeshToString: {
            mean_ms_max: 50.0,
            error_ratio_max: 0.10,
            alloc_bytes_max: 12000000
        },
        Optimize_MeshToString: {
            mean_ms_max: 50.0,
            error_ratio_max: 0.10,
            alloc_bytes_max: 12000000
        }
    }
}

/** Visual defaults */
const COLORS = {
    expectedBand: { border: "#000", fill: "#FEB019" },
    minMaxLine: "#00E396",
};




/** ------------------------------------
* Small utilities
* -------------------------------------*/


/**
* Return the config object for ApexCharts y‑axis annotations based on provided range.
* If both min and max are provided => shaded band. If only one => labeled line.
*/
function buildYAnnotations(min, max) {
    const ann = [];
    if (typeof min === "number" && typeof max === "number") {
        ann.push({
            y: min,
            y2: max,
            borderColor: COLORS.expectedBand.border,
            fillColor: COLORS.expectedBand.fill,
            opacity: 0.2,
            label: {
                borderColor: "#333",
                style: { fontSize: "10px", color: "#333", background: COLORS.expectedBand.fill },
                text: "Expected range",
            },
        });
    } else if (typeof min === "number") {
        ann.push({
            y: min,
            borderColor: COLORS.minMaxLine,
            label: {
                borderColor: COLORS.minMaxLine,
                style: { color: "#fff", background: COLORS.minMaxLine },
                text: "Min value",
            },
        });
    } else if (typeof max === "number") {
        ann.push({
            y: max,
            borderColor: COLORS.minMaxLine,
            borderWidth: 4,
            strokeDashArray: 10,
            label: {
                borderColor: COLORS.minMaxLine,
                style: { color: "#fff", background: COLORS.minMaxLine, fontSize: "12px" },
                text: "Max value",
            },
        });
    }
    return ann;
}



/**
* Compute y‑axis min/max with optional padding around raw values and explicit range.
* @param {number[]} values – data series values
* @param {{min?: number, max?: number}} explicit – explicit range (optional)
* @param {boolean} addMargin – whether to pad the axis around min/max
*/
function computeYAxisBounds(values, explicit = {}, addMargin = true) {
    const rawMin = Math.min(...values);
    const rawMax = Math.max(...values);


    let min = typeof explicit.min === "number" ? Math.min(explicit.min, rawMin) : rawMin;
    let max = typeof explicit.max === "number" ? Math.max(explicit.max, rawMax) : rawMax;


    if (addMargin) {
        const span = Math.max(1e-9, max - min); // avoid zero‑span axis
        const pad = span * 0.2; // 20% headroom/tailroom
        min = min - pad;
        max = max + pad;
    }


    return { min, max };
}



/**
* Lookup benchmark range by composite id "Suite.Method".
*/
function lookupRange(compositeId) {
    const [suite, ...rest] = String(compositeId).split(".");
    const method = rest.join(".");
    const entry = BENCHMARK_RANGES?.[suite]?.[method];
    if (!entry) return { min: undefined, max: undefined };
    return { min: entry.mean_ms_min, max: entry.mean_ms_max };
}



/** ------------------------------------
* DOM helpers
* -------------------------------------*/


/**
* Create a simple card wrapper with a title and an inner div that will host the chart.
* Returns the created chart container element.
*/
function createChartCard(parent, title, containerId) {
    const card = document.createElement("div");
    card.className = "w-full bg-white rounded-lg shadow-sm dark:bg-gray-800 p-4 md:p-6";


    const header = document.createElement("div");
    header.className = "flex justify-between";


    const titleEl = document.createElement("h2");
    titleEl.className = "text-2xl font-bold text-gray-900 dark:text-white pb-2";
    titleEl.textContent = title;


    header.appendChild(titleEl);
    card.appendChild(header);


    const container = document.createElement("div");
    container.id = containerId;
    card.appendChild(container);


    parent.appendChild(card);
    return container;
}




/** ------------------------------------
* Chart creation
* -------------------------------------*/


/**
* Render a line chart in the given containerId using ApexCharts.
*/
function renderLineChart({ containerId, values, categories, yRange, addMargin = true }) {
    if (!document.getElementById(containerId)) return;
    if (typeof ApexCharts === "undefined") return;


    const annotations = buildYAnnotations(yRange?.min, yRange?.max);
    const yBounds = computeYAxisBounds(values, yRange, addMargin);


    /** @type {import('apexcharts').ApexOptions} */
    const options = {
        series: [{ data: values }],
        chart: {
            height: "auto",
            type: "line",
            id: `chart-${containerId}`,
            zoom: { enabled: false },
            toolbar: { show: false },
        },
        annotations: { yaxis: annotations },
        dataLabels: { enabled: false },
        stroke: { curve: "straight" },
        labels: categories,
        xaxis: {
            type: "datetime",
            labels: {
                style: {
                    colors: ["#808080"]
                }
            }
        },
        yaxis: {
            decimalsInFloat: 0,
            min: yBounds.min,
            max: yBounds.max,
            labels: {
                style: {
                    colors: ["#808080"]
                }
            }

        },
    };


    const chart = new ApexCharts(document.getElementById(containerId), options);
    chart.render();
}

/** ------------------------------------
* Data extraction
* -------------------------------------*/


/**
* Convert BENCHMARK_DATA into a map keyed by benchmark name with arrays of {values, dates}.
* BENCHMARK_DATA number values are assumed to be presented in nanoseconds and will be converted to milliseconds.
*/
function extractSeriesFromBenchmarkData(benchmarkData) {
    const out = {};
    const entries = benchmarkData?.entries?.Benchmark ?? [];


    for (const commit of entries) {
        const date = commit.date; // assumed ISO string
        for (const bench of commit.benches ?? []) {
            const id = bench.name;
            if (!out[id]) out[id] = { values: [], dates: [] };
            out[id].values.push(bench.value / 1_000_000); // ns -> ms
            out[id].dates.push(date);
        }
    }


    return out;
}


/** ------------------------------------
* Bootstrapping
* -------------------------------------*/


function initBenchmarkCharts() {
    try {
        const root = document.getElementById("BenchmarkResults");
        if (!root) return;
        if (!window.BENCHMARK_DATA) return;


        const seriesById = extractSeriesFromBenchmarkData(window.BENCHMARK_DATA);


        for (const [benchmarkId, series] of Object.entries(seriesById)) {
            // Build card frame and container
            createChartCard(root, benchmarkId, benchmarkId);


            // Lookup optional expected range to annotate & pad y‑axis
            const range = lookupRange(benchmarkId);


            // Render the chart
            renderLineChart({
                containerId: benchmarkId,
                values: series.values,
                categories: series.dates,
                yRange: range,
                addMargin: true,
            });
        }
    } catch (err) {
        // Non‑fatal: keep the page usable even if one chart fails
        console.error("Failed to initialize benchmark charts:", err);
    }
}


// Kick things off once the DOM is ready
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initBenchmarkCharts);
} else {
    initBenchmarkCharts();
}