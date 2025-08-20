import json
import os
import sys


def load_json(path):
    with open(path, 'r') as f:
        return json.load(f)


def main():
    result_path = sys.argv[1] if len(sys.argv) > 1 else 'BenchmarkDotNet.Artifacts/results/SpeedBenchmarks-report.json'
    thresholds_path = os.getenv('BENCHMARK_THRESHOLDS', 'examples/Benchmark/benchmark_thresholds.json')

    results = load_json(result_path)
    thresholds = load_json(thresholds_path) if os.path.exists(thresholds_path) else {}

    benchmarks = {b['Method']: b for b in results.get('Benchmarks', [])}
    errors = []

    for method, limits in thresholds.items():
        bench = benchmarks.get(method)
        if bench is None:
            errors.append(f'Benchmark {method} not found in results')
            continue

        stats = bench.get('Statistics', {})
        memory = bench.get('Memory', {})

        mean_ms = stats.get('Mean', 0) / 1e6
        stderr = stats.get('StandardError', 0)
        error_ratio = (stderr / stats['Mean']) if stats.get('Mean') else 0
        alloc = memory.get('BytesAllocatedPerOperation')

        # Allow legacy single-value keys as maximum limits
        if 'mean_ms' in limits and 'mean_ms_max' not in limits:
            limits['mean_ms_max'] = limits['mean_ms']
        if 'error_ratio' in limits and 'error_ratio_max' not in limits:
            limits['error_ratio_max'] = limits['error_ratio']
        if 'alloc_bytes' in limits and 'alloc_bytes_max' not in limits:
            limits['alloc_bytes_max'] = limits['alloc_bytes']

        if 'mean_ms_min' in limits and mean_ms < limits['mean_ms_min']:
            errors.append(f'{method}: mean {mean_ms:.3f} ms below limit {limits["mean_ms_min"]} ms')
        if 'mean_ms_max' in limits and mean_ms > limits['mean_ms_max']:
            errors.append(f'{method}: mean {mean_ms:.3f} ms exceeds limit {limits["mean_ms_max"]} ms')
        if 'error_ratio_min' in limits and error_ratio < limits['error_ratio_min']:
            errors.append(f'{method}: error ratio {error_ratio:.4f} below limit {limits["error_ratio_min"]}')
        if 'error_ratio_max' in limits and error_ratio > limits['error_ratio_max']:
            errors.append(f'{method}: error ratio {error_ratio:.4f} exceeds limit {limits["error_ratio_max"]}')
        if alloc is not None:
            if 'alloc_bytes_min' in limits and alloc < limits['alloc_bytes_min']:
                errors.append(f'{method}: allocations {alloc} bytes below limit {limits["alloc_bytes_min"]}')
            if 'alloc_bytes_max' in limits and alloc > limits['alloc_bytes_max']:
                errors.append(f'{method}: allocations {alloc} bytes exceed limit {limits["alloc_bytes_max"]}')

    if errors:
        for e in errors:
            print(e, file=sys.stderr)
        sys.exit(1)
    else:
        print('Benchmark thresholds satisfied.')


if __name__ == '__main__':
    main()
