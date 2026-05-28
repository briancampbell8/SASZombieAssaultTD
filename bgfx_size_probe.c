/*
 * BGFX Struct Size Probe
 * Compile with: cl bgfx_size_probe.c /I<path_to_bgfx_headers> /link bgfx.lib
 * Or: gcc bgfx_size_probe.c -o bgfx_size_probe -I<path_to_bgfx_headers> -L. -lbgfx
 */
#include <stdio.h>
#include <stdint.h>

// Include BGFX headers - adjust path as needed
#include <bgfx/c99/bgfx.h>

int main() {
    printf("=== BGFX STRUCT SIZES ===\n");
    printf("sizeof(bgfx_init_t)=%zu\n", sizeof(bgfx_init_t));
    printf("sizeof(bgfx_platform_data_t)=%zu\n", sizeof(bgfx_platform_data_t));
    printf("sizeof(bgfx_resolution_t)=%zu\n", sizeof(bgfx_resolution_t));
    printf("sizeof(bgfx_init_limits_t)=%zu\n", sizeof(bgfx_init_limits_t));
    
    printf("\n=== BGFX INIT FIELD OFFSETS ===\n");
    bgfx_init_t init;
    printf("offset type=%zu\n", (size_t)&init.type - (size_t)&init);
    printf("offset vendorId=%zu\n", (size_t)&init.vendorId - (size_t)&init);
    printf("offset deviceId=%zu\n", (size_t)&init.deviceId - (size_t)&init);
    printf("offset capabilities=%zu\n", (size_t)&init.capabilities - (size_t)&init);
    printf("offset debug=%zu\n", (size_t)&init.debug - (size_t)&init);
    printf("offset profile=%zu\n", (size_t)&init.profile - (size_t)&init);
    printf("offset fallback=%zu\n", (size_t)&init.fallback - (size_t)&init);
    printf("offset platformData=%zu\n", (size_t)&init.platformData - (size_t)&init);
    printf("offset resolution=%zu\n", (size_t)&init.resolution - (size_t)&init);
    printf("offset limits=%zu\n", (size_t)&init.limits - (size_t)&init);
    printf("offset callback=%zu\n", (size_t)&init.callback - (size_t)&init);
    printf("offset allocator=%zu\n", (size_t)&init.allocator - (size_t)&init);
    
    printf("\n=== BGFX PLATFORM DATA FIELD OFFSETS ===\n");
    bgfx_platform_data_t pd;
    printf("offset ndt=%zu\n", (size_t)&pd.ndt - (size_t)&pd);
    printf("offset nwh=%zu\n", (size_t)&pd.nwh - (size_t)&pd);
    printf("offset context=%zu\n", (size_t)&pd.context - (size_t)&pd);
    printf("offset backBuffer=%zu\n", (size_t)&pd.backBuffer - (size_t)&pd);
    printf("offset backBufferDS=%zu\n", (size_t)&pd.backBufferDS - (size_t)&pd);
    printf("offset session=%zu\n", (size_t)&pd.session - (size_t)&pd);
    printf("offset type=%zu\n", (size_t)&pd.type - (size_t)&pd);
    
    return 0;
}
