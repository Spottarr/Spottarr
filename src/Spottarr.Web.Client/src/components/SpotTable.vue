<script setup lang="ts">
import { DownloadIcon } from 'mdi-vue3';
import prettyBytes from 'pretty-bytes';
import useSpots from '@/composables/useSpots';
import { useTimeAgo } from '@vueuse/core';
import type { ColumnDef, SortingState } from '@tanstack/vue-table';
import { FlexRender, getCoreRowModel, useVueTable } from '@tanstack/vue-table';
import type { Ref } from 'vue';
import { computed, h, onMounted, ref } from 'vue';
import type { SpotResponseDto } from '@/types/generated/spot-response-dto.ts';
import ExternalLinkButton from '@/components/ExternalLinkButton.vue';
import TableSortIcon from '@/components/TableSortIcon.vue';

const { spots, error, loading, fetchSpots, nzbUrl } = useSpots();

const columns: Ref<ColumnDef<SpotResponseDto>[]> = ref([
  {
    accessorKey: 'category',
    header: 'Category',
  },
  {
    accessorKey: 'title',
    header: 'Title',
  },
  {
    accessorKey: 'genre',
    header: 'Genre',
  },
  {
    accessorKey: 'spotter',
    header: 'Spotter',
  },
  {
    accessorKey: 'spottedAt',
    header: 'Age',
    cell: (props) => {
      return useTimeAgo(props.getValue() as Date).value;
    },
  },
  {
    accessorKey: 'bytes',
    header: 'Size',
    cell: (props) => {
      return prettyBytes(props.getValue() as number);
    },
  },
  {
    header: 'Actions',
    cell: (props) =>
      h(ExternalLinkButton, {
        url: nzbUrl(props.row.original.id),
        icon: DownloadIcon,
      }),
  },
]);

const sorting: Ref<SortingState> = ref([]);
const table = useVueTable({
  columns: columns.value,
  data: spots,
  getCoreRowModel: getCoreRowModel(),
  state: {
    sorting: sorting.value,
  },
  onSortingChange: (updater) => {
    const res = (sorting.value = typeof updater === 'function' ? updater(sorting.value) : updater);
    console.log(res);
    return res;
  },
});

const headers = computed(() => table.getFlatHeaders());
const rows = computed(() => table.getRowModel().rows);

onMounted(fetchSpots);
</script>
<template>
  <template v-if="error">{{ error }}</template>
  <template v-else-if="loading">
    <div>Loading...</div>
  </template>
  <template v-else-if="spots.length">
    <div class="overflow-x-auto">
      <table class="border-collapse table-auto w-full">
        <thead class="font-light bg-gray-100 dark:bg-slate-700 text-left">
          <tr class="border-b border-gray-200 dark:border-slate-600">
            <th
              v-for="header in headers"
              :key="header.id"
              :class="header.column.getCanSort() ? 'cursor-pointer select-none' : ''"
              @click="header.column.getToggleSortingHandler()?.($event)"
            >
              <span class="flex items-center space-x-4 w-full p-4">
                <span class="flex-1">
                  <flex-render :render="header.column.getIsSorted()" :props="header.getContext()" />
                </span>
                <table-sort-icon class="flex-none" :is-sorted="header.column.getIsSorted()" />
              </span>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="row in rows"
            :key="row.id"
            class="border-b last:border-0 dark:border-slate-600 even:bg-gray-50 odd:bg-white dark:even:bg-slate-700 dark:odd:bg-slate-800"
          >
            <td v-for="cell in row.getVisibleCells()" :key="cell.id" class="p-4">
              <flex-render :render="cell.column.columnDef.cell" :props="cell.getContext()" />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </template>
</template>
