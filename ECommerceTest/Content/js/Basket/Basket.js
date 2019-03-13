var BasketModel = {
    OrderDetailDeleteUrl: null,
    OrderDetailsUpdateUrl: null,
    PaymentInfoUrl: null,
    SavePromise: function () {
        return new Promise(function (Resolve) {
            var OrderDetails = new Array();

            $('.item').each(function (Index, Item) {
                Item = $(Item);
                var OrderDetailID = Item.attr('data-id');
                var ProductCount = Item.find('input').val();
                OrderDetails.push({ Key: OrderDetailID, Value: ProductCount });                
            });

            $.ajax({
                type: 'POST',
                url: BasketModel.OrderDetailsUpdateUrl,
                data: { OrderDetails: OrderDetails },
                dataType: 'json',
                success: function (res) {
                    if (res.IsSuccess) {
                        $('.js-product-count-badge').html(res.ProductTotalCount);
                        Resolve(true);
                    }
                    else {
                        $('[data-modal="alert"]').find('.text').html(res.Data);
                        $('[data-modal="alert"]').modal('show');
                    }
                },
                error: function (response) {
                    $('[data-modal="alert"]').find('.text').html(res.Data);
                    $('[data-modal="alert"]').modal('show');
                }
            });
        });
    },
    ReomoveItem: function ($this) {
        var OrderDetailID = $this.data('id');
        
        $.ajax({
            type: 'POST',
            url: BasketModel.OrderDetailDeleteUrl,
            data: {
                OrderDetailID: OrderDetailID
            },
            success: function (res) {
                if (res.IsSuccess) {
                    $this.slideUp(200, function () {
                        $(this).remove();
                    });
                    $('.js-total-price').html(res.Data.TotalPrice);
                    $('.js-product-count-badge').html(res.Data.ProductCountInBasket);                    
                    if (res.ProductCountInBasket <= 0) {
                        $('.js-product-count-badge').Hide();
                        $('.js-product-count-badge-container').Hide();
                    }
                }
            },
            error: function (response) {
                $('[data-modal="alert"]').find('.text').html(Globals.TextError);
                $('[data-modal="alert"]').modal('show');
            }
        });
    }
};

$(function () {
	
	//-- product count - allow only numbers
	$('.cart-grid .product-count input').keydown(function(e){		
		// Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
             // Allow: Ctrl+A
            (e.keyCode === 65 && e.ctrlKey === true) ||
             // Allow: Ctrl+C
            (e.keyCode === 67 && e.ctrlKey === true) ||
			 // Allow: Ctrl+V
            (e.keyCode === 86 && e.ctrlKey === true) ||
             // Allow: Ctrl+X
            (e.keyCode === 88 && e.ctrlKey === true) ||
             // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
                 // let it happen, don't do anything
                 return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
	});
	
	//-- product count - min/max val
    $('.cart-grid .product-count input').keyup(function () {		
        var value = $(this).val();

        if (value < 1) {
			value = 1;
		}
		if(value > 99){
			value = 99;
		}
        $(this).val(value);


        var gridItem = $(this).closest('.item');

        var itemPrice = gridItem.data('item-price').substring(1);
        var itemTotalPrice;
        var itemTotalPriceOld = gridItem.data('item-total-price').substring(1);
        var itemTotalPriceContainer = gridItem.find('.current-total span');

        var totalPriceContainer = $('.total-price .current-total span');
        var totalPrice = totalPriceContainer.html().substring(1);

        itemTotalPrice = (+itemPrice * 100 * +value / 100).toFixed(2);
        itemTotalPriceContainer.html('$' + itemTotalPrice);
        gridItem.data('item-total-price', itemTotalPrice);
        console.log(itemTotalPriceOld);

        totalPriceContainer.html('$' + (((totalPrice * 100) - (itemTotalPriceOld * 100) + (itemTotalPrice * 100)) / 100).toFixed(2));
        

	});
	
	//-- remove grid item
    $('.cart-grid  .js-remove-item').click(function (e) {
        e.preventDefault();
        var $this = $(this).closest('.item');
        BasketModel.ReomoveItem($this);        
    });

    //-- proceed to order recipient info
    $('.js-checkout-button').click(function () {
        BasketModel.SavePromise().then(function (IsOrderSaved) {
            if (IsOrderSaved) {
                window.location = BasketModel.PaymentInfoUrl;
            }
        });
    });

    $('.js-product-count').change(function () {
        BasketModel.SavePromise();
    });
});