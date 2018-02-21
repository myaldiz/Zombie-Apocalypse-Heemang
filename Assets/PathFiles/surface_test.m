%% Read The Surface
clear;
format long;
fileName = 'C:\Users\USER\Documents\Zombie_Apocalypse_Heemang\Assets\PathFiles\integrate.txt';
file = fopen(fileName, 'r');

my_surface = [];
while ~feof(file)
    line = fgetl(file);
    line_data = strsplit(line, ',');
    my_surface = [my_surface; str2double(line_data)];
end
fclose(file);

%% Show as an image
surf_thres = my_surface;
obstacle_index = surf_thres > (max(size(my_surface)) * 5);
surf_thres(obstacle_index) = 0;
norm_img = surf_thres ./ max(max(surf_thres));
norm_img(obstacle_index) = 1.0;
% big_img = imresize(norm_img, 10, 'nearest');

% figure
% imshow(big_img );
%% Plot the surface
% figure
% mesh(1: size(surf_thres, 2), 1:size(surf_thres, 1), surf_thres);
% 
% figure
% bar3(surf_thres);
%% Calculate Move Directions Without Gaussian filter
% [gx gy] = imgradientxy(norm_img, 'sobel');
% gx = gx/4.;
% gy = gy/4.;
% mag = gx.^2 + gy.^2;
% mag = sqrt(mag);
% gx_norm = gx ./ mag;
% gy_norm = gy ./ mag;
% 
% gx_norm(obstacle_index) = 0;
% gy_norm(obstacle_index) = 0;
% 
% figure
% quiver(1:100, 1:100, -gx_norm, -gy_norm);

%% Apply gausssian and gradient step by step
kernel_size = 3;
sigma = 3;
gaus_filt = fspecial('gaussian', kernel_size, sigma);
% 
% [gx gy] = imgradientxy(norm_img, 'sobel');
% gx = gx/4.;
% gy = gy/4.;
% 
% gx = imfilter(gx, gaus_filt);
% gy = imfilter(gy, gaus_filt);
% 
% mag = gx.^2 + gy.^2;
% mag = sqrt(mag);
% gx_norm = gx ./ mag;
% gy_norm = gy ./ mag;
% 
% gx_norm(obstacle_index) = 0;
% gy_norm(obstacle_index) = 0;
% 
%  figure
%  quiver(1: size(surf_thres, 2), 1:size(surf_thres, 1), -gx_norm, -gy_norm);


%% Calculate Directions with Gaussian applied
sobel_filt = fspecial('sobel');
sobel_filt = sobel_filt./4;

final_filt = imfilter(gaus_filt, sobel_filt);

gx = imfilter(norm_img, final_filt);
gy = imfilter(norm_img, final_filt');

mag = gx.^2 + gy.^2;
mag = sqrt(mag);
gx_norm = gx ./ mag;
gy_norm = gy ./ mag;

gx_norm(obstacle_index) = 0;
gy_norm(obstacle_index) = 0;

%figure
%quiver(1:100, 1:100, gx_norm, gy_norm);

%% Save to a file in similar format
fileName = 'C:\Users\USER\Documents\Zombie_Apocalypse_Heemang\Assets\PathFiles\vector_field.x';
file_x = fopen(fileName, 'w');
fileName = 'C:\Users\USER\Documents\Zombie_Apocalypse_Heemang\Assets\PathFiles\vector_field.y';
file_y = fopen(fileName, 'w');

for i = 1:size(my_surface, 1)

    for j = 1:(size(my_surface, 2) - 1)
        fprintf(file_x, '%f,', -gx_norm(i,j));   
        fprintf(file_y, '%f,', -gy_norm(i,j));
    end
    fprintf(file_x, '%f\n', -gx_norm(i,size(my_surface, 2)));
    fprintf(file_y, '%f\n', -gy_norm(i,size(my_surface, 2)));
end
fclose(file_x);
fclose(file_y);
