<script setup lang="ts">
import { DownloadIcon, SortIcon } from 'mdi-vue3';
import prettyBytes from 'pretty-bytes';
import useSpots from '@/composables/useSpots';
import { useTimeAgo } from '@vueuse/core';
import { onMounted } from 'vue';

const { spots, error, loading, fetchSpots, nzbUrl } = useSpots();

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
        <thead>
          <tr
            class="font-light border-b border-gray-200 dark:border-slate-600 bg-gray-100 dark:bg-slate-700 text-left"
          >
            <th class="">
              <a class="flex items-center space-x-4 w-full p-4" href="#">
                <span class="flex-1">Category</span>
                <SortIcon class="size-5 flex-none fill-gray-400 dark:fill-slate-400" />
              </a>
            </th>
            <th class="">
              <a class="flex items-center space-x-4 w-full p-4" href="#">
                <span class="flex-1">Title</span>
                <SortIcon class="size-5 flex-none fill-gray-400 dark:fill-slate-400" />
              </a>
            </th>
            <th class="">
              <a class="flex items-center space-x-4 w-full p-4" href="#">
                <span class="flex-1">Genre</span>
                <SortIcon class="size-5 flex-none fill-gray-400 dark:fill-slate-400" />
              </a>
            </th>
            <th class="">
              <a class="flex items-center space-x-4 w-full p-4" href="#">
                <span class="flex-1">Spotter</span>
                <SortIcon class="size-5 flex-none fill-gray-400 dark:fill-slate-400" />
              </a>
            </th>
            <th class="">
              <a class="flex items-center space-x-4 w-full p-4" href="#">
                <span class="flex-1">Age</span>
                <SortIcon class="size-5 flex-none fill-gray-400 dark:fill-slate-400" />
              </a>
            </th>
            <th class="">
              <a class="flex items-center space-x-4 w-full p-4" href="#">
                <span class="flex-1">Size</span>
                <SortIcon class="size-5 flex-none fill-gray-400 dark:fill-slate-400" />
              </a>
            </th>
            <th class="p-4">Links</th>
          </tr>
        </thead>
        <tbody>
          <tr
            class="border-b last:border-0 dark:border-slate-600 even:bg-gray-50 odd:bg-white dark:even:bg-slate-700 dark:odd:bg-slate-800"
            v-for="spot in spots"
            :key="spot.id"
          >
            <td class="p-4">{{ spot.category }}</td>
            <td class="p-4">{{ spot.title }}</td>
            <td class="p-4">{{ spot.genre }}</td>
            <td class="p-4">{{ spot.spotter }}</td>
            <td class="p-4">{{ useTimeAgo(spot.spottedAt) }}</td>
            <td class="p-4">{{ prettyBytes(spot.bytes) }}</td>
            <td class="p-4">
              <a class="" target="_blank" :href="nzbUrl(spot.id)">
                <button class="rounded-md dark:bg-slate-600 dark:hover:bg-slate-500 p-2">
                  <download-icon class="size-5" />
                </button>
              </a>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </template>
</template>
